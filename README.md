# Calculator

Принимает входную строку, содержащую математическое выражение (целые и десятично-дробные числа, знаки +, -, *, / и скобки) и выводит результат его вычисления

Для добавления новой операции нужно создать класс операции в Calculator.CountingService/Operations и реализовать в нем интерфейс IOperation

# ToDo:
- запилить возможность работы с отрицательными числами (например: -52 + 12 * (-1))
- убрать ограничение количества аргументов операций (сейчас их два. Каждая операция ожидает 2 числа: левое и правое)
