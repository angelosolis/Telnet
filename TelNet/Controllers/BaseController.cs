using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TelNet;
using TelNet.Repositories;

namespace TelNet.Controllers
{

    public class BaseController : Controller
    {
        public TelnetDBEntities _db;
        public BaseRepository<userCredentials> _userRepo;
        public BaseRepository<userRoles> _userRolesTable;
        public BaseRepository<userApplications> _userApplicationsRepo;
        public BaseRepository<notificationLogs> _notificationRepo;
        public BaseRepository<customerAccounts> _customerAccountsRepo;
        public BaseRepository<Folders> _foldersRepo;
        public BaseRepository<Files> _filesRepo;

        public BaseController()
        {
            _db = new TelnetDBEntities();
            _userRepo = new BaseRepository<userCredentials>();
            _userRolesTable = new BaseRepository<userRoles>();
            _userApplicationsRepo = new BaseRepository<userApplications>();
            _notificationRepo = new BaseRepository<notificationLogs>();
            _customerAccountsRepo = new BaseRepository<customerAccounts>();
            _foldersRepo = new BaseRepository<Folders>();
            _filesRepo = new BaseRepository<Files>();
        }

    }
}