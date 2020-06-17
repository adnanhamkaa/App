using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models
{
    public class ViewModelBase
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TForm">Form Model</typeparam>
    public class ViewModelBase<TForm> : ViewModelBase where TForm : FormModelBase
    {
        public TForm Form { get; set; }
    }
}