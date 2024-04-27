using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BigEcommerceApp.Tests.Models;
using Microsoft.Playwright;
using NUnit.Framework;
using Allure.NUnit.Attributes;
using Allure.NUnit;
using NUnit.Framework.Constraints;

//Основной класс с тестами

[AllureNUnit]
[TestFixture]
public class DodoTest : BaseTest
{
    //case_0
    [Retry(2)]
    [Test]
    [AllureTag("TestCountAndRegion")]
    public async Task TestCountAndRegion() {
        //Создание объекта страницы и попытка ее открыть
        var page = new MainPage(Page);
        try{
        await page.GotoAsync();
        await page.WaitForLoadStateAsync();        

        Assert.That(await page.GetLocator("//span[contains(text(), 'Додо Пицца')]")
            .IsVisibleAsync(), "Сайт не открылся"); //Если на сайте нету надписи "Додо Пицца" страница считается не открытой

        //Получение локаторов на все пиццы
        var pizzaLocators = await page.GetAllLocatorsAsync("//h2[contains(text(), 'Пиццы')]//..//article");
        //Подсчет кол-ва пицц
        Assert.That(pizzaLocators.Count > 0, "Пиццы не найдены");
        Console.WriteLine("Найдено " + pizzaLocators.Count + " пицц");

        //Получение региона
        Assert.That(await page.GetLocator("//a[contains(text(), 'Москва')]")
            .IsVisibleAsync(), "Регион не Москва");     
        Console.WriteLine("Регион " + await page.ReturnTextAsync(page.GetLocator("//a[contains(text(), 'Москва')]")));
        await page.SaveSreenchot(1);
        }catch(Exception){
            await page.SaveSreenchot(1);
        }
    }

    

    //case_1
    [Retry(2)]
    [Test]
    [AllureTag("TestPopupAndCart")]
    public async Task TestPopupAndCart() {
        //Создание объекта страницы и попытка ее открыть
        var page = new MainPage(Page);
        try{
        await page.GotoAsync();
        await page.WaitForLoadStateAsync();        

        Assert.That(await page.GetLocator("//span[contains(text(), 'Додо Пицца')]")
            .IsVisibleAsync(), "Сайт не открылся");  //Если на сайте нету надписи "Додо Пицца" страница считается не открытой

        Assert.That(await page
            .GetLocator("//h2[contains(text(), 'Пиццы')]")  //Проверка наличия блока с пиццами на главной странице
            .IsVisibleAsync(), "Нет пицц на главной странице"); 

        //Получение локаторов на все пиццы
        var pizzaLocators = await page
            .GetAllLocatorsAsync("//h2[contains(text(), 'Пиццы')]//..//article"); 
        
        //Генерирование нового порядкового номера пиццы
        var rand = new Random();
        int randPizza = rand.Next(pizzaLocators.Count);
        //Перегенерирование порядкового номера, если вместо кнопки выбрать какая-то другая
        while(await page.ReturnTextAsync(page
            .GetLocator("//h2[contains(text(), 'Пиццы')]//..//article[" + (randPizza + 1).ToString() + "]/footer/button")) != "Выбрать"){
            randPizza = rand.Next(pizzaLocators.Count);
        }

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

        //Поиск цены пиццы на главной странице
        String pizzaCostPath = "//h2[contains(text(), 'Пиццы')]//..//article[" + (randPizza + 1).ToString() + "]/footer/div";
        Assert.That(await page
            .GetLocator(pizzaCostPath)
            .IsVisibleAsync(), "Нет цены пиццы на главной странице"); 
        String pizzaCostFromMainPage = await page.ReturnTextAsync(page.GetLocator(pizzaCostPath));

        //Нажатие на кнопку 'Выбрать', если она существует
        String pizzaButtonPath = "//h2[contains(text(), 'Пиццы')]//..//article[" + (randPizza + 1).ToString() + "]/footer/button[contains(text(), 'Выбрать')]";
        Assert.That(await page
            .GetLocator(pizzaButtonPath)
            .IsVisibleAsync(), "Нет кнопки 'Выбрать'"); 
        await page.ClickElementAsync(page
            .GetLocator(pizzaButtonPath));

        //Поиск названия пиццы во всплывающем окне
        Assert.That(await page
            .GetLocator("//div[@class='popup-inner undefined']//h1")
            .IsVisibleAsync(), "Нет названия пиццы во всплывающем окне"); 
        String pizzaNameFromPopup = await page.ReturnTextAsync(page.GetLocator("//div[@class='popup-inner undefined']//h1"));

        //Сравнение названий пицц с главной страницы и из всплывающего окна
        Assert.That(pizzaNameFromMainPage == pizzaNameFromPopup,
            "Неверное название пиццы\nНа главной: " + pizzaNameFromMainPage + "\nВо всплывающем окне: " + pizzaNameFromPopup);
        Console.WriteLine("Верное название пиццы(" + pizzaNameFromMainPage + ")");

        //Нажатие на кнопку 'Маленькая', если она существует
        Assert.That(await page
            .GetLocator("//label[contains(text(), 'Маленькая')]")
            .IsVisibleAsync(), "Нет кнопки 'Маленькая'"); 
        await page.ClickElementAsync(page
            .GetLocator("//label[contains(text(), 'Маленькая')]"));

        //Поиск цены пиццы во всплывающем окне
        Assert.That(await page
            .GetLocator("//button[@data-testid='button_add_to_cart']//span[@class='money__value']")
            .IsVisibleAsync(), "Нет цены пиццы во всплывающем окне"); 
        String pizzaCostFromPopup = await page.ReturnTextAsync(page
            .GetLocator("//button[@data-testid='button_add_to_cart']//span[@class='money__value']"));

        //Сравнение цен пицц с главной страницы и из всплывающего окна
        Assert.That(pizzaCostFromMainPage.Contains(pizzaCostFromPopup),
            "Неверная цена пиццы\nНа главной: " + pizzaCostFromMainPage + "\nВо всплывающем окне: " + pizzaCostFromPopup);
        Console.WriteLine("Верная цена пиццы(" + pizzaCostFromPopup + ")");
        
        //Нажатие на кнопку 'Добавить в корзину'
        await page.ClickElementAsync(page
            .GetLocator("//button[@data-testid='button_add_to_cart']"));

        //Нажатие на кнопку 'Забрать из пиццерии'
        Assert.That(await page
            .GetLocator("//button[@data-testid='how_to_get_order_pickup_action']")
            .IsVisibleAsync(), "Нет кнопки 'Забрать из пиццерии'"); 
        await page.ClickElementAsync(page
            .GetLocator("//button[@data-testid='how_to_get_order_pickup_action']"));

        //Нажатие на кнопку 'Выбрать'
        Assert.That(await page
            .GetLocator("//button[@data-testid='menu_product_select']")
            .IsVisibleAsync(), "Нет кнопки 'Выбрать'"); 
        await page.ClickElementAsync(page
            .GetLocator("//button[@data-testid='menu_product_select']"));

        await page.PauseTest(1000); //Ожидание для подгрузки страницы
        //Поиск счетчика добавленных товаров
        Assert.That(await page
            .GetLocator("//div[@data-testid='cart-button__quantity']")
            .IsVisibleAsync(), "Нет кол-ва позиций в корзине"); 
        Console.WriteLine(await page.ReturnTextAsync(page.GetLocator("//div[@data-testid='cart-button__quantity']")) + " товаров в корзине");
        await page.SaveSreenchot(2);
        }catch(Exception){
            await page.SaveSreenchot(2);
        }
    }

    //case_2
    [Retry(2)]
    [Test]
    [AllureTag("TestTitlesAndPrices")]
    public async Task TestTitlesAndPrices() {
        //Создание объекта страницы и попытка ее открыть
        var page = new MainPage(Page);
        try{
        await page.GotoAsync();
        await page.WaitForLoadStateAsync();        

        //Если на сайте нету надписи "Додо Пицца" страница считается не открытой
        Assert.That(await page.GetLocator("//span[contains(text(), 'Додо Пицца')]")
            .IsVisibleAsync(), "Сайт не открылся");  

        //Проверка наличия блока с пиццами на главной странице
        Assert.That(await page
            .GetLocator("//h2[contains(text(), 'Пиццы')]")  
            .IsVisibleAsync(), "Нет пицц на главной странице"); 

        //Получение локаторов на все пиццы
        var pizzaLocators = await page
            .GetAllLocatorsAsync("//h2[contains(text(), 'Пиццы')]//..//article"); 

        var rand = new Random();
        List<int> addedPizzas = new List<int>(); //Номера пицц, уже добавленных в корзину
        List<String> addedPizzasNames = new List<String>(); //Имена пицц, уже добавленных в корзину

        await AddPizza(page, rand, pizzaLocators, addedPizzas, addedPizzasNames);
        
        //Нажатие кнопки 'Забрать из пиццерии'
        Assert.That(await page
            .GetLocator("//button[@data-testid='how_to_get_order_pickup_action']")
            .IsVisibleAsync(), "Нет кнопки 'Забрать из пиццерии'"); 
        await page.ClickElementAsync(page
            .GetLocator("//button[@data-testid='how_to_get_order_pickup_action']"));

        //Нажатие кнопки 'Выбрать'
        Assert.That(await page
            .GetLocator("//button[@data-testid='menu_product_select']")
            .IsVisibleAsync(), "Нет кнопки 'Выбрать'"); 
        await page.ClickElementAsync(page
            .GetLocator("//button[@data-testid='menu_product_select']"));

        await page.PauseTest(1000);  //Ожидание для подгрузки страницы
        
        //Отдельный цикл для добавления еще 4 пицц
        for (int i = 0; i < 4; i++) {
            await AddPizza(page, rand, pizzaLocators, addedPizzas, addedPizzasNames);   
            await page.PauseTest(1000);  //Ожидание для подгрузки страницы
        }

        //Поиск счетчика добавленных товаров
        Assert.That(await page
            .GetLocator("//div[@data-testid='cart-button__quantity']")
            .IsVisibleAsync(), "Нет кол-ва позиций в корзине"); 
        //Получение и проверка счетчика товаров в корзине
        String numOfPizzasInCart = await page.ReturnTextAsync(page.GetLocator("//div[@data-testid='cart-button__quantity']"));
        Console.WriteLine(numOfPizzasInCart + " товаров в корзине");
        Assert.That(numOfPizzasInCart == "5", "Неверное кол-во пицц в корзине");

        //Нажатие на кнопку 'Корзина'
        Assert.That(await page
            .GetLocator("//button[@data-testid='navigation__cart']")
            .IsVisibleAsync(), "Нет кнопки 'Корзина'"); 
        await page.ClickElementAsync(page
            .GetLocator("//button[@data-testid='navigation__cart']"));

        //Получение локаторов названий пицц в корзине
        var pizzaInCartNamesLocators = await page
            .GetAllLocatorsAsync("//div[@class ='scroll__view']//h3[@class!='title']"); 

        //Сравнение названий добавленных пицц с названиями в корзине
        foreach(ILocator locator in pizzaInCartNamesLocators){
            Assert.That(addedPizzasNames.Contains(await page
                .ReturnTextAsync(locator)), "Названия добавленных пицц не совпадают с выбранными");
        }
        Console.WriteLine("Названия добавленных пицц совпадают с выбранными");

        //Поиск итоговой стоимости товара в корзине
        Assert.That(await page
            .GetLocator("//h1[@class='cart-title']")
            .IsVisibleAsync(), "Нет итоговой стоимости заказа"); 
        String totalOrderCost = await page.ReturnTextAsync(page.GetLocator("//h1[@class='cart-title']"));
        int totalCostParsed, somePriceParsed;

        //Парсинг цены из строки
        totalOrderCost = totalOrderCost.Substring(totalOrderCost.IndexOf("на") + 3);
        int.TryParse(string.Join("", totalOrderCost.Where(c => char.IsDigit(c))), out totalCostParsed);

        //Получение цен всех пицц в корзине
        var pricesOfPizzas = await page.GetAllLocatorsAsync("//div[@class='current']");
        int totalPrice = 0;
        String somePrice;
        //Добавление цены каждой пиццы к переменной
        foreach(ILocator locator in pricesOfPizzas){
            somePrice = await page.ReturnTextAsync(locator);
            //Парсинг цены из строки
            int.TryParse(string.Join("", somePrice.Where(c => char.IsDigit(c))), out somePriceParsed);
            totalPrice += somePriceParsed;
        }
         
        //Сравнение цены в корзине и цены полученной сложением
        Assert.That(totalPrice == totalCostParsed,
            "Неверная итоговая цена\nЦена в корзине: " + totalCostParsed + "\nРеальная цена: " + totalPrice);
        Console.WriteLine("Верная итоговая стоимость заказа (" + totalCostParsed + ")");
        await page.SaveSreenchot(3);
        }catch(Exception){
            await page.SaveSreenchot(3);
        }
    }

    //case_3
    [Retry(2)]
    [Test]
    [AllureTag("TestСontacts")]
    public async Task TestСontacts() {
        //Создание объекта страницы и попытка ее открыть
        var page = new MainPage(Page);
        try{
        await page.GotoAsync();
        await page.WaitForLoadStateAsync();        

        //Если на сайте нету надписи "Додо Пицца" страница считается не открытой
        Assert.That(await page.GetLocator("//span[contains(text(), 'Додо Пицца')]")
            .IsVisibleAsync(), "Сайт не открылся");  

        //Нажание на кнопку 'Контакты'
        Assert.That(await page
            .GetLocator("//div[@id='react-app']//a[contains(text(), 'Контакты')]")
            .IsVisibleAsync(), "Нет кнопки 'Контакты'"); 
        await page.ClickElementAsync(page
            .GetLocator("//div[@id='react-app']//a[contains(text(), 'Контакты')]"));
        
        await page.PauseTest(1000);  //Ожидание для подгрузки страницы
        //Локаторы для контактов
        var contactLocators = await page.GetAllLocatorsAsync("//a[@class='contacts-pizzerias__information-desc']");
        Assert.That(contactLocators.Count > 0, "Нет контактов на странице");

        //Получение содержимого полей контактов
        String phoneNumber = await page.ReturnTextAsync(contactLocators[0]);
        String mail = await page.ReturnTextAsync(contactLocators[1]);
        
        //Проверка на правильность контактов
        Assert.That(phoneNumber.Equals("8 800 302-00-60"), "Номер телефона не совпадает");
        Console.WriteLine("Номер телефона совпадает");
        Assert.That(mail.Equals("feedback@dodopizza.com"), "Почта не совпадает");
        Console.WriteLine("Почта совпадает");
        await page.SaveSreenchot(4);
        }catch(Exception){
            await page.SaveSreenchot(4);
        }
    }
}