using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Data.Models
{
    public partial class Announcement
    {
        public Guid AnnouncementId { get; set; }
        public string AnnouncementContent { get; set; }
        public Guid ContestId { get; set; }

        public Contest Contest { get; set; }
    }
}
