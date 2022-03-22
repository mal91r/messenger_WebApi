«Сервис сообщений»

Task:

Разработать ASP.Net Core приложение для работы с сервисом сообщений.
Данная программа должна реализовывать функционал для просмотра
информации о пользователях и их сообщениях (в случае реализации
дополнительных критериев – и их добавления в систему).
Пользователь характеризуется именем и адресом электронной почты.
Электронная почта используется в качестве идентификатора, поэтому
является уникальной (не может быть двух пользователей с одинаковым
почтовым адресом).
Сообщение характеризуется темой, текстовым содержанием и, конечно же,
пользователем-отправителем и пользователем-получателем.
Программа должна обладать следующей функциональностью:
1. Механизм чтения и записи списка пользователей и сообщений из
соответствующих JSON-файлов (в соответствующие JSON-файлы).
Список пользователей хранится в упорядоченном виде, т.е. сортируется 
3
лексикографически по почтовому адресу (Email) по возрастанию.
Пользователи обладают двумя свойствами – string UserName, string
Email.
Сообщения обладают четырьмя свойствами: Subject, Message,
SenderId, ReceiverId.
Данный функционал должен использоваться обработчиками,
перечисленными ниже.
2. Реализовать обработчик, доступный по методу POST для инициализации
(т.е. первоначального заполнения) списка пользователей и списка
сообщений случайным образом (с использованием Random). С
использованием указанного обработчика заполнить списки.
3. Реализовать два обработчика, доступных только через метод GET:
a) для получения информации о пользователе по его идентификатору
(Email), учитывая, что при отсутствии пользователя код ответа должен
быть HTTP 404 (not found);
b) для получения всего списка пользователей.
4. Реализовать обработчик GET для получения списка сообщений по
идентификатору отправителя и получателя.
5. Реализовать обработчики GET для получения списка сообщений:
a) по идентификатору отправителя (получатель – любой);
b) по идентификатору получателя (отправитель – любой).
6. Использовать Swagger для получения автоматически генерируемой
документации по реализованным обработчикам.
7. [Дополнительный функционал] Предусмотреть возможность
регистрации новых пользователей: создать обработчик (POST),
добавляющий информацию о новом пользователе в систему (через поля
формы или JSON – решать вам).
8. [Дополнительный функционал] Предусмотреть возможность отправки
сообщений: создать обработчик (POST), добавляющий информацию о
новом сообщении. Предусмотреть проверку того, что отправитель и
получатель сообщения – зарегистрированные пользователи (т.е.
существуют в списке пользователей). Вернуть сообщение об ошибке,
если хотя бы одного из пользователей нет в списке (само сообщение при
этом не сохраняется).
9. [Дополнительный функционал] Доработать обработчик для получения
списка всех пользователей (п. 3b), реализовав поддержку функционала
для постраничной выборки – добавить поддержку параметров Limit и
Offset:
• int Limit – количество пользователей, которое необходимо вернуть
(максимальное);
• int Offset – порядковый номер пользователя, начиная с которого 
4
необходимо получать информацию (другими словами – количество
пользователей, которые необходимо пропустить с начала списка).
При отрицательных значениях Offset или неположительных значениях
Limit – возвращать HTTP 400 (bad request).
Список пользователей для определения порядкового номера
сортируется лексикографически по Email (по возрастанию).
Пример: для условного списка {a, b, c, d, e, f, g}:
- применение параметров Limit=2 и Offset=3 вернет список {d, e};
- применение параметров Limit=3 и Offset=5 вернет список {f, g}
