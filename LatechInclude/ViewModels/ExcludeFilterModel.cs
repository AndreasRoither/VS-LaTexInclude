using LatechInclude.HelperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatechInclude.Views.UserControls
{
    public class ExcludeFilterModel : ObservableObject, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "Exclude Filter";
            }
        }
    }
}
