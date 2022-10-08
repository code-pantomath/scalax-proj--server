﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalax_server
{
    internal class CONSTANTS
    {
        public static readonly string SERVER_ENDPOINT_URL = "https://scalax-server-01.herokuapp.com";

        public static readonly string PROJECT_DIR_PATH = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        public static readonly string WWWROOT_DIR_PATH = Path.Combine(PROJECT_DIR_PATH, "wwwroot");
        public static readonly string ACTIVE_CONS_TXTFILE_PATH = Path.Combine(PROJECT_DIR_PATH, "active_cons.txt");
        public static readonly string ACTIVE_TOKENS_TXTFILE_PATH = Path.Combine(PROJECT_DIR_PATH, "active_tks.txt");

    }
}