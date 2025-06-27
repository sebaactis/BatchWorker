using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchProcessing.Models.DTO
{
    public class ProcessExecutionDTO
    {
        public string ProcessName { get; set; } = string.Empty;
        public DateTime ProcessStartDate { get; set; }
        public DateTime ProcessEndDate { get; set; }
        public string ProcessDuration { get; set; } = string.Empty;
        public int ProcessState { get; set; }
        public int SuccessItems { get; set; }
        public int FailedItems { get; set; }
    }
}
