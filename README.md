# Использование
## Первоначальная настройка перед запуском тестов:
Установка браузеров:
```shell
pwsh bin/Debug/netX/playwright.ps1 install  
```

Перед запуском установить переменную для выполнения теста в режиме headed (в режиме headless все тесты падают на моменте загрузки страницы, потому что сайт ловит автоматические запросы):
```shell
 $env:HEADED=1
```

Запустить все тесты из консоли:
```shell
dotnet test
```

Чтобы заупстить тесты отдельно:
```shell
dotnet test --filter TestCountAndRegion
dotnet test --filter TestPopupAndCart
dotnet test --filter TestTitlesAndPrices
dotnet test --filter TestСontacts
```

## Для того, чтобы собрать отчет в Allure:
Необходимо сгенерировать результаты тестов:
```shell
npx playwright test
```

Затем сгенерировать отчет:
```shell
allure generate allure-results -o allure-report --clean
```

Для открытия отчета:
```shell
allure open allure-report
```


## P.S.
У меня не вышло подружиться с чем-то:
```
PS C:\hgjtu\WORK\DODO_AUTOTEST> npx playwright test
Error: No tests found
```
Долго пыталась это исправить, но не вышло, в теории все должно работать, как описано, но, видимо я чего-то не сделала
Также скриншоты в идеале должы делаться при помощи Allure, но, так как он у меня не сработал, реализовала при помощи try catch.