# EntityFramework and AspNet.Core example
Example application, demonstrating programming on C# whith EntityFramework and AspNet.Core

Этот код является примером полностью функционального приложения, которое выполняет функцию автоматизации аптечного склада.

Приложение представлено с целью помочь изучающим этот вопрос увидеть в работающем коде те приёмы и подходы, которые требуются для реализации подобного функционала.

Приложение является клиентом, работающим поверх EntityFramework. Хранение данных осуществляется в базе данных MS-SQL. 

Вторым  независимым компонентом приложения является WEB API интерфейс, позволяющий выполнять ряд операций через локальную или глобальную сеть путём REST запросов.

# Каталоги #

- __Aptechka.Data__       - файл тестовой базы данных для MS-SQL Server
- __Aptechka.Logs__       - файл лога базы данных
- __AptechkaAPI__         - каталог WEB API приложения
- __AptechkaWPF__         - каталог основного клиентского приложения на NET6.0 для WPF

Вы можете использовать этот код для самостоятельного изучения и экспериментов. Запрещается любое коммерческое использование кода или его части.
