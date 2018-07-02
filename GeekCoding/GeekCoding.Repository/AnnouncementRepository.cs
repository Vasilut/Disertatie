using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekCoding.Repository
{
    public class AnnouncementRepository: RepositoryBase<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(EvaluatorContext db):base(db)
        {

        }
    }
}
