﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UrlShortenerMVC.Startup))]
namespace UrlShortenerMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
