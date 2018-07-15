using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace PhysicsBookings.content.Booking
{
    public partial class DisplayBookingNotes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PhysicsExperiment p1 = new PhysicsExperiment();
                p1.Id = new Guid(Request.Params.Get("Id"));
                p1.Load();
                servercontent.InnerHtml = "<h3>Notes for: "+p1.ExperimentCode+" - "+p1.ExperimentDescription+"</h3><br/>"+p1.notes;
            }
        }
    }
}
