using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LatechInclude.HelperClass;

namespace LatechInclude.Views.UserControls
{
    public class WhiteListModel : ObservableObject, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "WhiteList";
            }
        }
    }
}
