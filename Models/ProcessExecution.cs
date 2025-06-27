using System.ComponentModel.DataAnnotations;

namespace BatchProcessing.Models
{
    public class ProcessExecution
    {
        [Key]
        public long ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public DateTime ProcessStartDate { get; set; }
        public DateTime ProcessEndDate { get; set; }
        public string ProcessDuration { get; set; } = string.Empty;
        public int ProcessState { get; set; }
        public int SuccessItems { get; set; }
        public int FailedItems { get; set; }

    }
}
