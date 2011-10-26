using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Configuration;

namespace ADInspector
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();

			ADQuery.LoadServerList();
		}

		private void btnQuery_Click(object sender, EventArgs e)
		{
			string strUserId = tbUserId.Text;

			tvValues.Nodes.Clear(); 
			
			if (strUserId.Equals(""))
				return;

			DataTable objTable = null;
			DirectoryEntry ADEntry = null;

			if (strUserId.Contains(","))
			{
				// got user full name, find user id and rest of data
				objTable = GetADTableWithFirstLastNames(strUserId);
				string[] strDomainUser;

				if (objTable.Rows.Count == 0)
				{
					// try composing the username and search it
					string[] strName = strUserId.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
					
					string strFirstInitial = "";
					if( strName.Length >= 2 )
						strFirstInitial = strName[1].Substring(0, 1);

					string strLastName = strName[0];

					strUserId = string.Concat(strFirstInitial, strLastName);
					strDomainUser = new string[] { "XXXX", strUserId };
				}
				else
				{
					// use the result in the table to search the rest of information
					string strUser = objTable.Rows[0]["Username"].ToString();
					string strDomain = objTable.Rows[0]["UserDomain"].ToString();
					strDomainUser = new string[] { strDomain, strUser };
				}

				ADEntry = GetEntriesFromAD(strDomainUser);
			}

			if( objTable == null || objTable.Rows.Count == 0)
			{
				// got user id, find user name and rest of data
				if (!strUserId.Contains("XXXX"))
					strUserId = string.Format("emsc\\{0}", strUserId);

				string[] strDomainUser = strUserId.Split('\\');

				string strFullName = "";
				ADEntry = GetEntriesFromAD(strDomainUser);
				try
				{
					strFullName = ADEntry.Properties["FullName"].Value.ToString();
				}
				catch (Exception excpt)
				{
					tvValues.Nodes.Add("NOT FOUND.");
				}

				objTable = GetADTableWithFirstLastNames(strFullName);
			}

			// display
			DisplayMainADEntries(objTable);

			DisplayBasicADEntries(ADEntry);
		}

		private static DirectoryEntry GetEntriesFromAD(string[] strDomainUser)
		{
			string strADEntry = "";

			if (strDomainUser.Length == 2)
				// get full name from the user id
				strADEntry = string.Format("WinNT://{0}/{1}", strDomainUser[0], strDomainUser[1]);
			else
				strADEntry = string.Format("WinNT://XXXX/{0}", strDomainUser[0]);

			DirectoryEntry ADEntry = new DirectoryEntry(strADEntry);
			return ADEntry;
		}

		private static DataTable  GetADTableWithFirstLastNames(string strUserId)
		{
			string[] strName = strUserId.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (strName.Length < 2)
				return new DataTable();
			string strFirstName = strName[1], strLastName = strName[0];
			return ADQuery.SelectUser("EMSC", strLastName, strFirstName);
		}

		private void DisplayMainADEntries(DataTable objTable)
		{
			if (objTable.Rows.Count != 0)
				foreach (DataColumn objDC in objTable.Columns)
				{
					string strNode = string.Format("{0}: {1}", objDC.ColumnName, objTable.Rows[0][objDC.ColumnName].ToString());
					tvValues.Nodes.Add(objDC.ColumnName, strNode);
				}
		}

		private void DisplayBasicADEntries(DirectoryEntry ADEntry)
		{
			try
			{
				if (ADEntry.Properties.Count == 0)
					return;
			}
			catch (Exception excpt)
			{
				return;
			}

			foreach (PropertyValueCollection objValues in ADEntry.Properties)
			{
				string strNode = string.Format("{0}: {1}", objValues.PropertyName, objValues.Value.ToString());
				tvValues.Nodes.Add(objValues.PropertyName, strNode);
			}
		}

		private void tbUserId_KeyPress(object sender, KeyPressEventArgs e)
		{
			if( e.KeyChar.Equals( '\r' ) )
				btnQuery_Click( sender, new EventArgs() );
		}


	}
}
