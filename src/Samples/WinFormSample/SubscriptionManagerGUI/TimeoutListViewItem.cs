namespace SubscriptionManagerGUI
{
    using System;
    using System.Windows.Forms;
    using MassTransit.Util;

    public class TimeoutListViewItem :
        ListViewItem
    {
        public TimeoutListViewItem(Tuple<Guid, DateTime> item)
        {
            //what is used to search
            //now you don't have to show the GUID
            this.Name = item.Key.ToString();
            
            //time scheduled
            this.SubItems.Add(item.Value.ToLocalTime().ToString());

            //time received
            this.SubItems.Add(DateTime.Now.ToLocalTime().ToString());
        }
    }
}