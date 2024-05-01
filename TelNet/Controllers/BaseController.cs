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

        public BaseController()
        {
            _db = new TelnetDBEntities();
            _userRepo = new BaseRepository<userCredentials>();
            _userRolesTable = new BaseRepository<userRoles>();
        }

    }
}