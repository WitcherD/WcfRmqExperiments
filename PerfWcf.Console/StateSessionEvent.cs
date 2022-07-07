using System;

namespace PerfWcf.Console
{
    public delegate void StateLocalSessionEventHandler(object sender, StateSessionEventArgs e);

    [Serializable]
    public class StateSessionEventArgs : EventArgs
    {
        #region Fields
        private string _UserName = ""; //Globals.CurrentUser.UserName;
        private int _UserId = 0;//Globals.CurrentUser.ID;
        private string _CampaignIdentifier;
        private bool _IsUserInitiatedKillSession;           //BBB KillSession
        public string WebSessionId { get; set; }
        #endregion;

        #region Accessors
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        public int UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        public string CampaignIdentifier
        {
            get { return _CampaignIdentifier; }
            set { _CampaignIdentifier = value; }
        }

        public bool IsUserInitiatedKillSession              //BBB KillSession
        {
            get { return _IsUserInitiatedKillSession; }
            set { _IsUserInitiatedKillSession = value; }
        }

        public object Info1 { get; set; }

        public object Info2 { get; set; }

        public object Info3 { get; set; }

        public object Info4 { get; set; }
        #endregion

        #region Lifecycle
        public StateSessionEventArgs()
        {
        }

        public StateSessionEventArgs(string UserName, int UserId)
        {
            this.UserName = UserName;
            this.UserId = UserId;
        }

        public StateSessionEventArgs(string UserName)
        {
            this.UserName = UserName;
        }

        public StateSessionEventArgs(string UserName, string WebSessionId)
        {
            this.UserName = UserName;
            this.WebSessionId = WebSessionId;
        }
        public StateSessionEventArgs(object Info1)
        {
            this.Info1 = Info1;
        }

        public StateSessionEventArgs(object Info1, bool UserInitiatedKillSession)            //BBB KillSession
        {
            this.Info1 = Info1;
            this.IsUserInitiatedKillSession = UserInitiatedKillSession;
        }

        public StateSessionEventArgs(object Info1, object Info2)
        {
            this.Info1 = Info1;
            this.Info2 = Info2;
        }

        public StateSessionEventArgs(string UserName, int UserId, object Info1)
        {
            this.UserName = UserName;
            this.UserId = UserId;
            this.Info1 = Info1;
        }

        public StateSessionEventArgs(string UserName, int UserId, object Info1, object Info2)
        {
            this.UserName = UserName;
            this.UserId = UserId;
            this.Info1 = Info1;
            this.Info2 = Info2;
        }

        public StateSessionEventArgs(string UserName, int UserId, object Info1, object Info2, object Info3)
        {
            this.UserName = UserName;
            this.UserId = UserId;
            this.Info1 = Info1;
            this.Info2 = Info2;
            this.Info3 = Info3;
        }
        public StateSessionEventArgs(string UserName, int UserId, object Info1, object Info2, object Info3, object Info4)
        {
            this.UserName = UserName;
            this.UserId = UserId;
            this.Info1 = Info1;
            this.Info2 = Info2;
            this.Info3 = Info3;
            this.Info4 = Info4;
        }
        #endregion

        #region Methods
        #endregion
    }
}
