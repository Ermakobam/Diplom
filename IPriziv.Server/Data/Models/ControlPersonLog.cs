using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.Data.Models
{
    [Table("T_ControlPersonStatusLog")]
    public class ControlPersonLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Слияние из другого контроля
        /// </summary>
        public bool IsMerged { get; set; }
        /// <summary>
        /// Тип события
        /// </summary>
        public ControlStatusLogType LogType { get; set; }
        /// <summary>
        /// Данные состояния
        /// </summary>
        public string State { get; set; }

        [ForeignKey("ControlPerson")]
        public int ControlPersonId { get; set; }

        public ControlPerson ControlPerson { get; set; }
        public ControlPersonLog()
        {
            CreatedAt = DateTime.Now;
        }
    }
    public enum ControlStatusLogType
    {
        /// <summary>
        /// Создание персоны
        /// </summary>
        Create,
        /// <summary>
        /// Редактирование персоны
        /// </summary>
        Update,

        /// <summary>
        /// Отправка телеграммы
        /// </summary>
        SendTelegram,

        /// <summary>
        /// Создание связанного контроля
        /// </summary>
        CreateControlRecruit,
        /// <summary>
        /// Обновление связанного контроля
        /// </summary>
        UpdateControlRecruit,
        /// <summary>
        /// Удаление связанного контроля
        /// </summary>
        DeleteControlRecruit,

        /// <summary>
        /// Удаление связи с контролем
        /// </summary>
        BindControlRecruit,
        /// <summary>
        /// Связь с контролем
        /// </summary>
        UnbindControlRecruit,

        /// <summary>
        /// Связь с призывником
        /// </summary>
        BindRecruit,

        /// <summary>
        /// Слияние персон
        /// </summary>
        Merge
    }
}
