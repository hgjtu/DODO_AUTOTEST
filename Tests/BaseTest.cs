using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BigEcommerceApp.Tests.Models;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

public class BaseTest : PageTest
{
 //пыталась переопределить настройки headless режима 
 //не вышло к сожалению :(   

    //Метод добавления пиццы в корзину
    protected async Task AddPizza(MainPage page, Random rand, List<ILocator> pizzaLocators, List<int> addedPizzas, List<String> addedPizzasNames){
        //Генерирование нового порядкового номера пиццы
        int randPizza = rand.Next(pizzaLocators.Count);
        //Перегенерирование порядкового номера, если пицца с таким номером уже добавлялась
        //или вместо кнопки 'Выбрать' какая-то другая
        while(addedPizzas.Contains(randPizza) || await page.ReturnTextAsync(page
            .GetLocator("//h2[contains(text(), 'Пиццы')]//..//article[" + (randPizza + 1).ToString() + "]/footer/button")) != "Выбрать"){
            randPizza = rand.Next(pizzaLocators.Count);
        }

        addedPizzas.Add(randPizza);//Добавление номера пиццы в массив
        //Выбор пути для названия пиццы (потому что они в разных местах на сайте)
        String pizzaNamePath = "//h2[contains(text(), 'Пиццы')]//..//article[" + (randPizza + 1).ToString() + "]//div[@data-gtm-id]/a";
        try{
            await page.GetLocator(pizzaNamePath).IsVisibleAsync();
        }catch(Exception){
            pizzaNamePath = "//h2[contains(text(), 'Пиццы')]//..//article[" + (randPizza + 1).ToString() + "]//div[@data-gtm-id]";
        }

            //Поиск названия пиццы на главной странице
        Assert.That(await page
            .GetLocator(pizzaNamePath)
            .IsVisibleAsync(), "Нет названия пиццы на главной странице"); 
        String pizzaNameFromMainPage = await page.ReturnTextAsync(page.GetLocator(pizzaNamePath));
        addedPizzasNames.Add(pizzaNameFromMainPage);  //Добавление названия пиццы в массив

        //Нажатие кнопки 'Выбрать'
        String pizzaButtonPath = "//h2[contains(text(), 'Пиццы')]//..//article[" + (randPizza + 1).ToString() + "]/footer/button[contains(text(), 'Выбрать')]";
        Assert.That(await page
            .GetLocator(pizzaButtonPath)
            .IsVisibleAsync(), "Нет кнопки 'Выбрать'"); 
        await page.ClickElementAsync(page
            .GetLocator(pizzaButtonPath));

        //Нажатие кнопки 'Добавить в корзину'
        Assert.That(await page
            .GetLocator("//button[@data-testid='button_add_to_cart']")
            .IsVisibleAsync(), "Нет кнопки добавить в корзину"); 
        await page.ClickElementAsync(page
            .GetLocator("//button[@data-testid='button_add_to_cart']"));    
    }
}