using System;
using System.Data;
using NLog;

namespace Tools.Extensions
{
    public static class DbTransactionExtensions
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static bool TryRollback(this IDbTransaction transaction)
        {
            try
            {
                transaction.Rollback();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при откатывании транзакции");
                return false;
            }
        }
    }
}