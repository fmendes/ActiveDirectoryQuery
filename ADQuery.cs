using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.DirectoryServices;

namespace ADInspector
{
	class ADQuery
	{

		private static string sObjectClass = "User";
		private static string m_sDomain;
		private static Dictionary<string, string> ServerList;

		public static string GetFullName( string strUserId )
		{
			if( ServerList.Count == 0 )
				LoadServerList();

			string sLDAPPath = string.Format( "LDAP://{0}/DC=XXXX,DC=root01,DC=org", ServerList["XXXX"] );
			string strFullName = "";
			DirectoryEntry objDE = null;
			try
			{
				objDE = new DirectoryEntry(sLDAPPath);

				DirectorySearcher objDS = new DirectorySearcher( objDE );

				// get the LDAP filter string based on selections
				string strFilter = string.Format("(|(&(objectClass=User)(sAMAccountName={0})))", strUserId);
				objDS.Filter = strFilter;
				objDS.ReferralChasing = ReferralChasingOption.None;

				//start searching
				SearchResultCollection objSRC = objDS.FindAll();

				try
				{
					if (objSRC.Count != 0)
					{
						// grab the first search result
						SearchResult objSR = objSRC[0];

						string strFirstName = objSR.Properties[ "givenName" ][ 0 ].ToString();
						string strLastName	= objSR.Properties[ "sn" ][ 0 ].ToString();
						strFullName			= string.Concat( strLastName, ", ", strFirstName );
					}
				}
				catch (Exception e)
				{
					// ignore errors
				}

				objSRC.Dispose();
				objDS.Dispose();
			}
			catch (Exception e)
			{
				// ignore errors
			}

			return strFullName;
		}

		public static void LoadServerList()
		{
			ServerList = new Dictionary<string, string>();
			ServerList.Add("XXXX", "XXXX.root01.org");
			ServerList.Add("XXXX", "XXXX.root01.org");
			...
		}

		public static DataTable SelectUser(string strDomain, string sLastNameSearch, string sFirstNameSearch)
		{
			DataTable objTable = CreateNetworkUserTable();
			objTable.Rows.Clear();

			return LookForUserInDomain(objTable, strDomain, sLastNameSearch, sFirstNameSearch);
		}

		private static DataTable LookForUserInDomain(DataTable objTable, string strDomain, string sLastNameSearch, string sFirstNameSearch)
		{
			string sUID = null, sPwd = null;

			string serverName = ServerList[strDomain.ToUpper()];
			string sLDAPPath = "LDAP://" + serverName + "/DC=" + strDomain + ",DC=root01,DC=org";

			objTable = GetLDAPUserInfo(objTable, sLDAPPath, sUID, sPwd, sObjectClass, sLastNameSearch, sFirstNameSearch);

			return objTable;
		}

		public static System.Data.DataTable GetLDAPUserInfo(DataTable objTable, string sLDAPPath, string sUserName, string sPassword,
					string sLDAPUserObjectClass, string sLastNameSearchFilter,
					string sFirstNameSearchFilter)
		{
			try
			{
				DirectoryEntry de;
				// get the domain server for this computer
				if ((sUserName != null) & (sPassword != null))
				{
					de = new DirectoryEntry(sLDAPPath, sUserName, sPassword, AuthenticationTypes.Secure);
				}
				else
				{
					// de = new DirectoryEntry("LDAP://XXXX/ou=membergroup1,o=plumtree");
					de = new DirectoryEntry(sLDAPPath);
				}


				m_sDomain = de.Name.Substring(3);
				DirectorySearcher ds = new DirectorySearcher(de);
				// get the LDAP filter string based on selections
				ds.Filter = GetFilterString(
						sLDAPUserObjectClass, sLastNameSearchFilter, sFirstNameSearchFilter);

				ds.ReferralChasing = ReferralChasingOption.None;

				//start searching
				SearchResultCollection src = ds.FindAll();

				try
				{
					foreach (SearchResult sr in src)
						AddObjectToTable(objTable, sr, sLDAPUserObjectClass);
				}
				catch (Exception e)
				{
					throw e;
				}
				src.Dispose();
				ds.Dispose();

				return objTable;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private static void AddObjectToTable(DataTable objTable, SearchResult sr, string sLDAPUserObjectClass)
		{
			try
			{
				DirectoryEntry de = sr.GetDirectoryEntry();
				// before we add the name, remove the CN= from name
				// if (String.Compare(de.SchemaClassName, "User", true)==0)
				if (String.Compare(de.SchemaClassName, sLDAPUserObjectClass, true) == 0)
				{

					//Added by muthu
					DirectoryEntry found = sr.GetDirectoryEntry();
					//					string sUID =   GetPropertyString(found, "uid"); //user id
					string sUID = GetPropertyString(found, "sAMAccountName"); // sAMAccountName
					string sLName = GetPropertyString(found, "sn"); //last name
					string sFName = GetPropertyString(found, "givenName"); //givenName
					string sPhone = GetPropertyString(found, "TelephoneNumber"); //TelephoneNumber
					string sEmail = GetPropertyString(found, "mail"); //email
					string sFullName = String.Concat(sFName, " ", sLName);

					string sAppUID = "-1";
					objTable.Rows.Add(new object[] { sAppUID, m_sDomain, sUID, sLName, sFName, sPhone, sEmail
						, GetPropertyString(found, "department")
						, GetPropertyString(found, "company")
						, GetPropertyString(found, "title")
						, GetPropertyString(found, "physicalDeliveryOfficeName")
						, sFullName
						, GetPropertyString(found, "manager")
						, GetPropertyString(found, "streetAddress")
						, GetPropertyString(found, "lastLogon")
						, GetPropertyString(found, "lastLogoff")						
							
						});
					//ApplicationUser_ID UserDomain Username LastName FirstName Phone Email
					//Department Company Title Office DisplayName
				}
			}
			catch (Exception e)
			{
				throw e;
			}

		}

		private static string GetPropertyString(DirectoryEntry Entry, string PropertyName)
		{
			try
			{
				if (Entry.Properties[PropertyName].Count > 0)
				{
					return Entry.Properties[PropertyName][0].ToString();
				}
				return "(not set)";
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private static string GetFilterString(string sLDAPUserObjectClass,
			string sLastNameSearchFilter, string sFirstNameSearchFilter)
		{
			try
			{
				// form the filter string for directory search
				string filter = "";
				if (sLastNameSearchFilter == null || sLastNameSearchFilter == "")
					sLastNameSearchFilter = "*";
				if (sFirstNameSearchFilter == null || sFirstNameSearchFilter == "")
					sFirstNameSearchFilter = "*";

				String strResult = String.Format(
					"(&(objectClass={0})(givenname={1})(sn={2}))",
					sLDAPUserObjectClass, sFirstNameSearchFilter, sLastNameSearchFilter);

				filter += strResult;

				// add all the above filter strings
				return "(|" + filter + ")";
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private static DataTable CreateNetworkUserTable()
		{
			DataTable objTable = new DataTable("NetworkUsers");

			objTable.Columns.Add(new DataColumn("ApplicationUser_ID", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("UserDomain", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("Username", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("LastName", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("FirstName", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("Phone", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("Email", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("Department", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("Company", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("Title", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("Office", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("manager", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("DisplayName", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("streetAddress", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("lastLogon", typeof(System.String)));
			objTable.Columns.Add(new DataColumn("lastLogoff", typeof(System.String)));

			return objTable;
		}
	}
}
