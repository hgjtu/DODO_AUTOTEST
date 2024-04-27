using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace BigEcommerceApp.Tests.Models;

public class BasePage
{
  protected IPage page;

  public BasePage(IPage _page) {
    this.page = _page;
  }
  //Переопределение метода открытия страницы
  public async Task GotoAsync() {
    await page.GotoAsync("https://dodopizza.ru/moscow",
      new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
  }

  //Переопределение метода ожидания загрузки страницы
  public async Task WaitForLoadStateAsync(){
      await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
  }

  //Переопределение метода получения локатора
  public ILocator GetLocator(String _path){
    return page.Locator(_path);
  }

  //Переопределение метода ожидания
  public async Task PauseTest(uint Ms){
    await page.WaitForTimeoutAsync(Ms);
  }

  //Метод нажатия на кнопку
  public async Task ClickElementAsync(ILocator _locator) {
    await _locator.ClickAsync();
    Console.WriteLine("Клик на: " + _locator.ToString());
  }

  //Метод получения текста
  public async Task<String> ReturnTextAsync(ILocator _locator){
    String innerText = await _locator.InnerTextAsync();
    Console.WriteLine("Текст: " + innerText + "\nИз: " + _locator.ToString());
    return innerText;
  }

  //Метод получения всех локаторов соответствующих XPath
  public async Task<List<ILocator>> GetAllLocatorsAsync(String _path){
    return (await page.Locator(_path).AllAsync()).ToList();
  }

  //Переопределение метода скриншота
  public async Task SaveSreenchot(int n){
    var screenshot = await page.ScreenshotAsync(new PageScreenshotOptions
    {
      Path = $"../../../Screenshots/Screenshot_{n}.png"
    });
  }
}