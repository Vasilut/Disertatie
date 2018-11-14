using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.ViewModels
{
    public class ContestAnnouncementViewModel
    {
        public Guid Contestid { get; set; }
        public Guid AnnouncementId { get; set; }
        public string AnnouncementContent { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
