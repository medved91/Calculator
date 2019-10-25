namespace Calculator.CountingService.Operations
{
    public interface IOperation
    {
        /// <summary>
        /// Символ или порядок символов, определяющий операцию
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Приоритет операции
        /// </summary>
        OperationPriority Priority { get; }

        /// <summary>
        /// Функция, используемая для подсчета операции
        /// </summary>
        double Count(double left, double right);
    }
}