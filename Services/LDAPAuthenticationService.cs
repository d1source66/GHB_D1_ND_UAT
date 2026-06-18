
using GHB_D1.Code.Util;
using GHB_D1.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace GHB_D1.Services
{
    public class LDAPAuthenticationService
    {
        private string _domain;
        Loger _logSys = null;
        string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
        iniConnection _iniCon = null;
        string StartPath = AppDomain.CurrentDomain.BaseDirectory;
        public LDAPAuthenticationService(string domain)
        {
            _domain = domain;
            _logSys = new Loger();
            _iniCon = new iniConnection();
            _iniCon.iniFile = StartPath + "GHBConf.ini";
            //******************ITDAPI******************
            _iniCon.ldap1 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "authen1");
            _iniCon.ldap2 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "authen2");
            _iniCon.itldap1 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "itAuth1");
            _iniCon.itldap2 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "itAuth2");
            _iniCon.itldap3 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "itAuth3");
            _iniCon.itUrl = ModConf.ReadIni(_iniCon.iniFile, "ldap", "itUrl");
            _iniCon.authenUrl = ModConf.ReadIni(_iniCon.iniFile, "ldap", "authenUrl");
            _iniCon.codeUrl = ModConf.ReadIni(_iniCon.iniFile, "ldap", "codeUrl");
            _iniCon.userldap1 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "userAuth");
            //******************digitaldev*******************
            _iniCon.itldap1_v2 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "itAuth1_v2");
            _iniCon.itldap2_v2 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "itAuth2_v2");
            _iniCon.itldap3_v2 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "itAuth3");
            _iniCon.itUrl_v2 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "itUrl_v2");//token            
            _iniCon.authenUrl_v2 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "authenUrl_v2");//sign in            
            _iniCon.codeUrl_v2 = ModConf.ReadIni(_iniCon.iniFile, "ldap", "codeUrl_v2");//code
        }

        public bool IsAuthenticated(string username, string password)
        {
            bool isAuthenticated = false;

            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, _domain))
                {
                    isAuthenticated = context.ValidateCredentials(username, password);
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                _logSys.WriteProcessLogFile(_strPathFile, "Error IsAuthenticated : " + ex.Message.ToString());
            }

            return isAuthenticated;
        }

        public string GetUserDisplayName(string username)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, _domain))
                using (var user = UserPrincipal.FindByIdentity(context, username))
                {
                    if (user != null)
                    {
                        
                        return user.DisplayName;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserDisplayName : " + ex.Message.ToString());
            }

            return null;
        }

        public UserModels GetUserByUserName(string username)
        {
            try
            {
                DirectoryEntry direct_entry = GetDirectoryEntry();
                DirectorySearcher direct_search = new DirectorySearcher();
                direct_search.SearchRoot = direct_entry;
                direct_search.PropertiesToLoad.Add("name");
                direct_search.PropertiesToLoad.Add("samaccountname");
                direct_search.PropertiesToLoad.Add("userPrincipalName");
                direct_search.PropertiesToLoad.Add("givenName");
                direct_search.PropertiesToLoad.Add("mail");
                direct_search.PropertiesToLoad.Add("department");
                direct_search.Filter = "(&(objectCategory=User)(objectClass=person)(samaccountname=" + username + "*))";
                SearchResultCollection result_col = direct_search.FindAll();
                _logSys.WriteProcessLogFile(_strPathFile, "result_col : " + result_col.Count);
                if (result_col.Count > 0)
                {
                    foreach (SearchResult sr in result_col)
                    {
                        UserModels obj = new UserModels();
                        // Using the index zero (0) is required!
                        if (sr.Properties["name"].Count > 0)
                        {
                            obj.USER_FIRSTNAME = sr.Properties["name"][0].ToString();
                            obj.USER_LASTNAME = sr.Properties["name"][0].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "sr.Properties['name'] : " + sr.Properties["name"][0].ToString());
                        }
                        if (sr.Properties["samaccountname"].Count > 0 && sr.Properties["userPrincipalName"].Count > 0)
                        {
                            obj.USER_LOGIN = sr.Properties["samaccountname"][0].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "sr.Properties['samaccountname'] : " + sr.Properties["userPrincipalName"][0].ToString());
                        }
                        if (sr.Properties["givenName"].Count > 0)
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "sr.Properties['givenName'] : " + sr.Properties["givenName"][0].ToString());
                        }
                        if (sr.Properties["mail"].Count > 0)
                        {
                            obj.USER_EMAIL = sr.Properties["mail"][0].ToString();
                            _logSys.WriteProcessLogFile(_strPathFile, "sr.Properties['mail'] : " + sr.Properties["mail"][0].ToString());
                        }
                        if (sr.Properties["department"].Count > 0)
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "sr.Properties['department'] : " + sr.Properties["department"][0].ToString());
                        }
                        return obj;
                    }
                }
                //using (var context = new PrincipalContext(ContextType.Domain, _domain))
                //using (var user = UserPrincipal.FindByIdentity(context, username))
                //{
                //    if (user != null)
                //    {
                //        UserModels obj = new UserModels();
                //        obj.USER_LOGIN = "DisplayName:"+user.DisplayName+ " UserPrincipalName:" + user.UserPrincipalName;
                //        obj.USER_FIRSTNAME = user.Name;
                //        obj.USER_LASTNAME = user.Surname;
                //        obj.USER_EMAIL = user.EmailAddress;

                //        return obj;

                //    }
                //}
            }
            catch (Exception ex)
            {
                // Log or handle exception
                _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName : " + ex.Message.ToString());
            }

            return null;
        }

        public ApiPostResponse GetUserByUserName1(UserPostRequest userReq)
        {
            try
            {
                ApiPostResponse ldapUser = new ApiPostResponse();
                ldapUser = HttpRequest5(userReq);
                return ldapUser;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName1 : " + ex.Message.ToString());
            }

            return null;
        }

        public bool GetUserByUserName2(UserPostRequest userReq)
        {
            bool result = false;
            try
            {
                ApiPostResponse2 ldapUser2 = new ApiPostResponse2();
                ldapUser2 = HttpRequestV2(userReq);
                //return null;
                if (ldapUser2.fname != null && ldapUser2.fname != "")
                {
                    result = true;
                }
                else
                {
                    result = false;
                    _logSys.WriteProcessLogFile(_strPathFile, "ไม่พบบัญชีผู้ใช้ " + userReq.username + " ในระบบ LDAP !");
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
            }

            return result;
        }

        public ApiPostResponse2 GetUserByUserName3(UserPostRequest userReq)
        {
            ApiPostResponse2 ldapUser = new ApiPostResponse2();
            try
            {
                ldapUser = HttpRequestV2(userReq);
                //return ldapUser;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName3 : " + ex.Message.ToString());
            }

            return ldapUser;
        }

        public ApiPostResponse2 GetUserByUserNameV3(UserPostRequest userReq, String empCode)
        {
            ApiPostResponse2 ldapUser = new ApiPostResponse2();
            try
            {
                ldapUser = HttpRequestV3(userReq, empCode);//AD Emp/Code
                //ldapUser = HttpRequestV4(userReq, empCode);//with mockup                
                //return ldapUser;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName3 : " + ex.Message.ToString());
            }

            return ldapUser;
        }
        //HttpRequestV3_2
        public ApiPostResponse2 GetUserByUserNameV3_2(UserPostRequest userReq, String empCode)
        {
            ApiPostResponse2 ldapUser = new ApiPostResponse2();
            try
            {
                ldapUser = HttpRequestV3_2(userReq, empCode);//AD Emp/Code
                //ldapUser = HttpRequestV4(userReq, empCode);//with mockup                
                //return ldapUser;
            }
            catch (Exception ex)
            {
                // Log or handle exception
                _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName3 : " + ex.Message.ToString());
            }

            return ldapUser;
        }

        public ApiPostResponse2 GetUserByUserName4(UserPostRequest userReq)
        {
            ApiPostResponse2 ldapUser2 = new ApiPostResponse2();
            try
            {
                ldapUser2 = HttpRequestV2(userReq); 
                //ldapUser2 = HttpRequestV2_test(userReq);
            }
            catch (Exception ex)
            {
                // Log or handle exception
                _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
            }

            return ldapUser2;
        }

        public DirectoryEntry GetDirectoryEntry()
        {
            string ldap_p = "LDAP://" + _domain;
            DirectoryEntry direct_entry = new DirectoryEntry(ldap_p, _iniCon.ldap1, _iniCon.ldap2);//กำหนด LDAP Server 
            direct_entry.AuthenticationType = AuthenticationTypes.Secure;
            _logSys.WriteProcessLogFile(_strPathFile, "direct_entry : " + direct_entry.Path);
            return direct_entry;
        }

        public string GetPropertyValue(SearchResult sr, string propertyName)
        {
            string ret = string.Empty;

            if (sr.Properties[propertyName].Count > 0)
                ret = sr.Properties[propertyName][0].ToString();

            return ret;
        }

        //Vesion5 Emp/User
        public ApiPostResponse HttpRequest5(UserPostRequest userReq)
        {
            ApiPostResponse ldapUser = new ApiPostResponse();
            _logSys.WriteProcessLogFile(_strPathFile, "<<< Start HttpRequest5 >>> ");
            _logSys.WriteProcessLogFile(_strPathFile, "Get Token Step ");
            dynamic tokendata = null;
            dynamic userdata = null;
            //************** oAuth/Token ***************
            using (var client = new HttpClient())
            {
                try
                {
                    var endPoint = new Uri(_iniCon.itUrl);
                    var result = client.GetAsync(endPoint).Result;
                    _logSys.WriteProcessLogFile(_strPathFile, "client.GetAsync(endPoint).Result : " + result);
                    var json = result.Content.ReadAsStringAsync();
                    _logSys.WriteProcessLogFile(_strPathFile, "result.Content.ReadAsStringAsync() : " + json);

                    var tokenRequest = new ApiPostRequest
                    {
                        username = _iniCon.itldap1,
                        password = _iniCon.itldap2,
                        granttype = _iniCon.itldap3
                    };
                    var newpostjson = JsonConvert.SerializeObject(tokenRequest);
                    _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + newpostjson);
                    var playload = new StringContent(newpostjson, Encoding.UTF8, "application/json");
                    _logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + playload);
                    var result1 = client.PostAsync(endPoint, playload).Result.Content.ReadAsStringAsync().Result;
                    _logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,playload) : " + result1);
                    tokendata = JsonConvert.DeserializeObject<dynamic>(result1);
                    _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + tokendata);
                }
                catch (Exception ex)
                {
                    _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName : " + ex.Message.ToString());
                }
            }
            _logSys.WriteProcessLogFile(_strPathFile, "<<< End HttpRequest5 >>>  ");



            //************** Emp/Auth(User) ***************
            if (null != tokendata)
            {
                _logSys.WriteProcessLogFile(_strPathFile, "*** Begin verified user *** ");
                var user_endPoint = new Uri(_iniCon.authenUrl);
                _logSys.WriteProcessLogFile(_strPathFile, "user_endPoint : " + _iniCon.authenUrl);
                using (var client2 = new HttpClient())
                {
                    try
                    {
                        var userRequest = new UserPostRequest
                        {
                            username = userReq.username,
                            password = userReq.password
                        };
                        var user_newpostjson = JsonConvert.SerializeObject(userRequest);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + user_newpostjson + " / " + tokendata.token);
                        var user_playload = new StringContent(user_newpostjson, Encoding.UTF8, "application/json");
                        string tk = tokendata.token;
                        client2.DefaultRequestHeaders.Add("X-Auth-Token", tk);
                        _logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + user_playload + " / " + user_playload.Headers);
                        var result1 = client2.PostAsync(user_endPoint, user_playload).Result.Content.ReadAsStringAsync().Result;
                        _logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,playload) : " + result1);
                        userdata = JsonConvert.DeserializeObject<dynamic>(result1);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + userdata);
                        if (null != userdata)
                        {
                            ldapUser.empCode = userdata.empCode;
                            ldapUser.fullName = userdata.fullName;
                            ldapUser.grade = userdata.grade;
                            ldapUser.email = userdata.email;
                            ldapUser.departmentCode = userdata.departmentCode;
                            ldapUser.department = userdata.department;
                            ldapUser.employeeTypeCode = userdata.employeeTypeCode;
                            ldapUser.employeeType = userdata.employeeType;
                            ldapUser.division = userdata.division;
                            ldapUser.position = userdata.position;
                            ldapUser.hubCode = userdata.hubCode;
                            ldapUser.hubName = userdata.hubName;
                            ldapUser.deCode = userdata.deCode;
                            ldapUser.deName = userdata.deName;
                            ldapUser.solCode = userdata.solCode;
                            ldapUser.solName = userdata.solName;
                            _logSys.WriteProcessLogFile(_strPathFile, "add User Details  " + ldapUser.fullName);
                            _logSys.WriteProcessLogFile(_strPathFile, "Show Details userdata.empCode : " + userdata.empCode + " / userdata.fullName : " + userdata.fullName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "This is not User ldap.  ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName : " + ex.Message.ToString());
                    }

                }
                _logSys.WriteProcessLogFile(_strPathFile, "*** End verified user *** ");
            }

            return ldapUser;

        }

        //Vesion2 login user
        public ApiPostResponse2 HttpRequestV2(UserPostRequest userReq)//20240913
        {
            ApiPostResponse2 ldapUser = new ApiPostResponse2();
            _logSys.WriteProcessLogFile(_strPathFile, "<<< Start Sup HttpRequestV2 >>> ");
            _logSys.WriteProcessLogFile(_strPathFile, "Get Token Step ");
            dynamic tokendata = null;
            dynamic userdata = null;
            UserPostCodeRequest codeR = new UserPostCodeRequest();

            //************** oAuth/Token ***************
            using (var client = new HttpClient())
            {
                try
                {
                    var endPoint = new Uri(_iniCon.itUrl_v2);
                    //var result = client.GetAsync(endPoint).Result;
                    _logSys.WriteProcessLogFile(_strPathFile, "Uri(endPoint) : " + endPoint);
                    //var json = result.Content.ReadAsStringAsync();
                    //_logSys.WriteProcessLogFile(_strPathFile, "result.Content.ReadAsStringAsync() : " + json);

                    var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", _iniCon.itldap3_v2),
                        new KeyValuePair<string, string>("username", _iniCon.itldap1_v2),
                        new KeyValuePair<string, string>("password", _iniCon.itldap2_v2)
                    };

                    // สร้าง FormUrlEncodedContent จากพารามิเตอร์
                    HttpContent content = new FormUrlEncodedContent(parameters);
                    _logSys.WriteProcessLogFile(_strPathFile, "FormUrlEncodedContent : " + parameters);
                    // กำหนด Headers (ถ้ามี)
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                    // ส่ง POST Request ไปยัง API
                    //HttpResponseMessage response = await client.PostAsync(endPoint, content);
                    var result1 = client.PostAsync(endPoint, content).Result.Content.ReadAsStringAsync().Result;
                    _logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,content) : " + result1);
                    tokendata = JsonConvert.DeserializeObject<dynamic>(result1);//.Content.ReadAsStringAsync();
                    _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + tokendata);
                    //codeR.emp_code = "41830";
                }
                catch (Exception ex)
                {
                    _logSys.WriteProcessLogFile(_strPathFile, "Error HttpRequestV2 : " + ex.Message.ToString());
                }
            }

            //************** SignIn/Auth(User) ***************
            if (null != tokendata && userReq.username != "" && userReq.password != "")
            {
                _logSys.WriteProcessLogFile(_strPathFile, "*** Begin verified user *** ");
                var user_endPoint = new Uri(_iniCon.authenUrl_v2);
                _logSys.WriteProcessLogFile(_strPathFile, "user_endPoint : " + _iniCon.authenUrl);
                using (var client2 = new HttpClient())
                {
                    try
                    {
                        var userRequest = new UserPostRequest
                        {
                            username = userReq.username,
                            password = userReq.password
                        };
                        var user_newpostjson = JsonConvert.SerializeObject(userRequest);
                        //_logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + user_newpostjson + " / " + tokendata.access_token);
                        var user_playload = new StringContent(user_newpostjson, Encoding.UTF8, "application/json");
                        _logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + user_playload + " / " + user_playload.Headers);
                        string authorization = tokendata.access_token;
                        // Set the authorization header with a Bearer token  
                        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
                        _logSys.WriteProcessLogFile(_strPathFile, "authorization: " + tokendata.token_type + " " + tokendata.access_token);
                        //client2.DefaultRequestHeaders.Add("X-Auth-Token", authorization);
                        var result1 = client2.PostAsync(user_endPoint, user_playload).Result.Content.ReadAsStringAsync().Result;
                        _logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,playload) : " + result1);
                        userdata = JsonConvert.DeserializeObject<dynamic>(result1);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + userdata);
                        var chkStu = userdata.status.ToString();                        
                        if (chkStu.Trim().ToLower() != "false")
                        {
                            var dt = userdata.data;

                            ldapUser.emp_code = dt[0].emp_code;
                            ldapUser.fname = dt[0].fname;
                            ldapUser.lname = dt[0].lname;
                            ldapUser.id_card = dt[0].id_card;
                            ldapUser.birth_date = dt[0].birth_date;
                            ldapUser.grade = dt[0].grade;
                            ldapUser.emp_type = dt[0].emp_type;
                            ldapUser.position_id = dt[0].position_id;
                            ldapUser.position_name = dt[0].position_name;
                            ldapUser.email = dt[0].email;
                            ldapUser.tel = dt[0].tel;
                            ldapUser.org_level = dt[0].org_level;
                            ldapUser.comp_objid = dt[0].comp_objid;
                            ldapUser.comp_code = dt[0].comp_code;
                            ldapUser.comp_short_name = dt[0].comp_short_name;
                            ldapUser.comp_name = dt[0].comp_name;
                            ldapUser.group_objid = dt[0].group_objid;
                            ldapUser.group_code = dt[0].group_code;
                            ldapUser.group_short_name = dt[0].group_short_name;
                            ldapUser.group_name = dt[0].group_name;
                            ldapUser.field_objid = dt[0].field_objid;
                            ldapUser.field_code = dt[0].field_code;
                            ldapUser.field_short_name = dt[0].field_short_name;
                            ldapUser.field_name = dt[0].field_name;
                            ldapUser.dept_objid = dt[0].dept_objid;
                            ldapUser.dept_code = dt[0].dept_code;
                            ldapUser.dept_short_name = dt[0].dept_short_name;
                            ldapUser.dept_name = dt[0].dept_name;
                            ldapUser.center_objid = dt[0].center_objid;
                            ldapUser.center_code = dt[0].center_code;
                            ldapUser.center_short_name = dt[0].center_short_name;
                            ldapUser.center_name = dt[0].center_name;
                            ldapUser.zone_objid = dt[0].zone_objid;
                            ldapUser.zone_code = dt[0].zone_code;
                            ldapUser.zone_short_name = dt[0].zone_short_names;
                            ldapUser.zone_name = dt[0].zone_name;
                            ldapUser.division_objid = dt[0].division_objid;
                            ldapUser.division_code = dt[0].division_code;
                            ldapUser.division_short_name = dt[0].division_short_name;
                            ldapUser.division_name = dt[0].division_name;
                            ldapUser.sol_objid = dt[0].sol_objid;
                            ldapUser.sol_code = dt[0].sol_code;
                            ldapUser.sol_short_name = dt[0].sol_short_name;
                            ldapUser.sol_name = dt[0].sol_name;
                            ldapUser.sub_sol_objid = dt[0].sub_sol_objid;
                            ldapUser.sub_sol_code = dt[0].sub_sol_code;
                            ldapUser.sub_sol_short_name = dt[0].sub_sol_short_name;
                            ldapUser.sub_sol_name = dt[0].sub_sol_name;
                            ldapUser.start_date = dt[0].start_date;
                            ldapUser.stop_date = dt[0].stop_date;
                            ldapUser.last_action = dt[0].last_action;
                            ldapUser.sol_instead_code = dt[0].sol_instead_code;
                            ldapUser.sol_instead_name = dt[0].sol_instead_name;
                            ldapUser.sol_hq = dt[0].sol_hq;
                            ldapUser.plans_000 = dt[0].plans_000;
                            ldapUser.plansname_000 = dt[0].plansname_000;
                            ldapUser.org_000 = dt[0].org_000;
                            ldapUser.solid_000 = dt[0].solid_000;
                            ldapUser.orgname_000 = dt[0].orgname_000;
                            ldapUser.org_748 = dt[0].org_748;
                            ldapUser.solid_748 = dt[0].solid_748;
                            ldapUser.orgname_748 = dt[0].orgname_748;
                            ldapUser.create_date = dt[0].create_date;
                            ldapUser.update_date = dt[0].update_date;
                            _logSys.WriteProcessLogFile(_strPathFile, "add User Details  " + ldapUser.fname);
                            _logSys.WriteProcessLogFile(_strPathFile, "Show Details userdata.empCode : " + userdata.emp_Code + " / userdata.lName : " + userdata.lName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "This is not User ldap.  ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
                    }
                }
            }

            //************** Emp/Code ***************
            /*
            if (null != tokendata && codeR.emp_code != "")
            {
                _logSys.WriteProcessLogFile(_strPathFile, "*** Begin verified with code *** ");
                var user_endPoint = new Uri(_iniCon.codeUrl_v2);
                _logSys.WriteProcessLogFile(_strPathFile, "user_endPoint : " + _iniCon.codeUrl_v2);
                using (var client2 = new HttpClient())
                {
                    try
                    {
                        var userCodeRequest = new UserPostCodeRequest
                        {
                            emp_code = codeR.emp_code
                        };
                        var user_newpostjson = JsonConvert.SerializeObject(userCodeRequest);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + user_newpostjson + " / " + tokendata.access_token);
                        var user_playload = new StringContent(user_newpostjson, Encoding.UTF8, "application/json");
                        _logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + user_playload + " / " + user_playload.Headers);
                        string authorization = tokendata.access_token;
                        // Set the authorization header with a Bearer token  
                        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
                        _logSys.WriteProcessLogFile(_strPathFile, "authorization: " + tokendata.token_type + " " + tokendata.access_token);
                        //client2.DefaultRequestHeaders.Add("X-Auth-Token", authorization);
                        var result1 = client2.PostAsync(user_endPoint, user_playload).Result.Content.ReadAsStringAsync().Result;
                        _logSys.WriteProcessLogFile(_strPathFile, "client2.PostAsync(user_endPoint,user_playload) : " + result1);
                        userdata = JsonConvert.DeserializeObject<dynamic>(result1);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + userdata);
                        if (null != userdata)
                        {
                            ldapUser.emp_code = userdata.emp_code;
                            ldapUser.fname = userdata.fname;
                            ldapUser.lname = userdata.lname;
                            ldapUser.id_card = userdata.id_card;
                            ldapUser.birth_date = userdata.birth_date;
                            ldapUser.grade = userdata.grade;
                            ldapUser.emp_type = userdata.emp_type;
                            ldapUser.position_id = userdata.position_id;
                            ldapUser.position_name = userdata.position_name;
                            ldapUser.email = userdata.email;
                            ldapUser.tel = userdata.tel;
                            ldapUser.org_level = userdata.org_level;
                            ldapUser.comp_objid = userdata.comp_objid;
                            ldapUser.comp_code = userdata.comp_code;
                            ldapUser.comp_short_name = userdata.comp_short_name;
                            ldapUser.comp_name = userdata.comp_name;
                            ldapUser.group_objid = userdata.group_objid;
                            ldapUser.group_code = userdata.group_code;
                            ldapUser.group_short_name = userdata.group_short_name;
                            ldapUser.group_name = userdata.group_name;
                            ldapUser.field_objid = userdata.field_objid;
                            ldapUser.field_code = userdata.field_code;
                            ldapUser.field_short_name = userdata.field_short_name;
                            ldapUser.field_name = userdata.field_name;
                            ldapUser.dept_objid = userdata.dept_objid;
                            ldapUser.dept_code = userdata.dept_code;
                            ldapUser.dept_short_name = userdata.dept_short_name;
                            ldapUser.dept_name = userdata.dept_name;
                            ldapUser.center_objid = userdata.center_objid;
                            ldapUser.center_code = userdata.center_code;
                            ldapUser.center_short_name = userdata.center_short_name;
                            ldapUser.center_name = userdata.center_name;
                            ldapUser.zone_objid = userdata.zone_objid;
                            ldapUser.zone_code = userdata.zone_code;
                            ldapUser.zone_short_name = userdata.zone_short_names;
                            ldapUser.zone_name = userdata.zone_name;
                            ldapUser.division_objid = userdata.division_objid;
                            ldapUser.division_code = userdata.division_code;
                            ldapUser.division_short_name = userdata.division_short_name;
                            ldapUser.division_name = userdata.division_name;
                            ldapUser.sol_objid = userdata.sol_objid;
                            ldapUser.sol_code = userdata.sol_code;
                            ldapUser.sol_short_name = userdata.sol_short_name;
                            ldapUser.sol_name = userdata.sol_name;
                            ldapUser.sub_sol_objid = userdata.sub_sol_objid;
                            ldapUser.sub_sol_code = userdata.sub_sol_code;
                            ldapUser.sub_sol_short_name = userdata.sub_sol_short_name;
                            ldapUser.sub_sol_name = userdata.sub_sol_name;
                            ldapUser.start_date = userdata.start_date;
                            ldapUser.stop_date = userdata.stop_date;
                            ldapUser.last_action = userdata.last_action;
                            ldapUser.sol_instead_code = userdata.sol_instead_code;
                            ldapUser.sol_instead_name = userdata.sol_instead_name;
                            ldapUser.sol_hq = userdata.sol_hq;
                            ldapUser.plans_000 = userdata.plans_000;
                            ldapUser.plansname_000 = userdata.plansname_000;
                            ldapUser.org_000 = userdata.org_000;
                            ldapUser.solid_000 = userdata.solid_000;
                            ldapUser.orgname_000 = userdata.orgname_000;
                            ldapUser.org_748 = userdata.org_748;
                            ldapUser.solid_748 = userdata.solid_748;
                            ldapUser.orgname_748 = userdata.orgname_748;
                            ldapUser.create_date = userdata.create_date;
                            ldapUser.update_date = userdata.update_date;
                            _logSys.WriteProcessLogFile(_strPathFile, "add User Details  " + ldapUser.fname);
                            _logSys.WriteProcessLogFile(_strPathFile, "Show Details userdata.empCode : " + userdata.emp_Code + " / userdata.lName : " + userdata.lName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "This is not User ldap.  ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
                    }
                }
            }
            */
            return ldapUser;
        }

        public ApiPostResponse2 HttpRequestV2_test(UserPostRequest userReq)//20240913
        {
            ApiPostResponse2 ldapUser = new ApiPostResponse2();
            _logSys.WriteProcessLogFile(_strPathFile, "<<< Start Sup HttpRequestV2 >>> ");
            _logSys.WriteProcessLogFile(_strPathFile, "Get Token Step ");
            dynamic tokendata = null;
            dynamic userdata = null;
            UserPostCodeRequest codeR = new UserPostCodeRequest();
            string result6 = "{\r\n  \"status\": true,\r\n  \"message\": \"พบข้อมูลจำนวน 1 record\",\r\n  \"data\": [\r\n   {\r\n\t\t\"emp_code\": \"40022\",\r\n\t\t\"fname\": \"ชื่อพนักงาน 40022\",\r\n\t\t\"lname\": \"นามสกุลพนักงาน 40022\",\r\n\t\t\"id_card\": \"\",\r\n\t\t\"birth_date\": \"\",\r\n\t\t\"grade\": \"9\",\r\n\t\t\"emp_type\": \"2\",\r\n\t\t\"position_id\": \"60002721\",\r\n\t\t\"position_name\": \"พนักงานคอมพิวเตอร์อาวุโส\",\r\n\t\t\"email\": \"SUCHEEWA.K@GHB.CO.TH\",\r\n\t\t\"tel\": \"22022266\",\r\n\t\t\"org_level\": \"600\",\r\n\t\t\"comp_objid\": \"50000001\",\r\n\t\t\"comp_code\": \"D9100\",\r\n\t\t\"comp_short_name\": \"ธอส.\",\r\n\t\t\"comp_name\": \"ธนาคารอาคารสงเคราะห์\",\r\n\t\t\"group_objid\": \"50005238\",\r\n\t\t\"group_code\": \"D9100\",\r\n\t\t\"group_short_name\": \"กง.ทส.\",\r\n\t\t\"group_name\": \"กลุ่มงานเทคโนโลยีสารสนเทศ\",\r\n\t\t\"field_objid\": \"50004657\",\r\n\t\t\"field_code\": \"D9100\",\r\n\t\t\"field_short_name\": \"สง.พรท.\",\r\n\t\t\"field_name\": \"สายงานพัฒนาระบบดิจิทัล\",\r\n\t\t\"dept_objid\": \"50000261\",\r\n\t\t\"dept_code\": \"88855\",\r\n\t\t\"dept_short_name\": \"พด.\",\r\n\t\t\"dept_name\": \"ฝ่ายพัฒนาระบบดิจิทัลเซอร์วิส\",\r\n\t\t\"center_objid\": \"\",\r\n\t\t\"center_code\": \"\",\r\n\t\t\"center_short_name\": \"\",\r\n\t\t\"center_name\": \"\",\r\n\t\t\"zone_objid\": \"\",\r\n\t\t\"zone_code\": \"\",\r\n\t\t\"zone_short_name\": \"\",\r\n\t\t\"zone_name\": \"\",\r\n\t\t\"division_objid\": \"50000345\",\r\n\t\t\"division_code\": \"88855\",\r\n\t\t\"division_short_name\": \"พด.(สสท.)\",\r\n\t\t\"division_name\": \"ส่วนสนับสนุนและทดสอบระบบดิจิทัล\",\r\n\t\t\"sol_objid\": \"\",\r\n\t\t\"sol_code\": \"\",\r\n\t\t\"sol_short_name\": \"\",\r\n\t\t\"sol_name\": \"สำนักงานใหญ่\",\r\n\t\t\"sub_sol_objid\": \"\",\r\n\t\t\"sub_sol_code\": \"\",\r\n\t\t\"sub_sol_short_name\": \"\",\r\n\t\t\"sub_sol_name\": \"\",\r\n\t\t\"start_date\": \"2012-06-05T00:00:00\",\r\n\t\t\"stop_date\": \"9999-12-31T00:00:00\",\r\n\t\t\"last_action\": null,\r\n\t\t\"sol_instead_code\": \"1001\",\r\n\t\t\"sol_instead_name\": \"กรุงเทพมหานคร\",\r\n\t\t\"sol_hq\": \"1000\",\r\n\t\t\"plans_000\": \"0\",\r\n\t\t\"plansname_000\": \"\",\r\n\t\t\"org_000\": \"\",\r\n\t\t\"solid_000\": \"\",\r\n\t\t\"orgname_000\": \"\",\r\n\t\t\"org_748\": \"\",\r\n\t\t\"solid_748\": \"\",\r\n\t\t\"orgname_748\": \"\",\r\n\t\t\"create_date\": \"2024-09-05T15:45:20.457\",\r\n\t\t\"update_date\": \"2024-09-05T15:45:20.457\"\r\n\t}\r\n  ]\r\n}";
            string result7 = "{\r\n  \"status\": true,\r\n  \"message\": \"พบข้อมูลจำนวน 1 record\",\r\n  \"data\": [\r\n   {\r\n\t\t\"emp_code\": \"02568\",\r\n\t\t\"fname\": \"ชื่อพนักงาน 02568\",\r\n\t\t\"lname\": \"นามสกุลพนักงาน 02568\",\r\n\t\t\"id_card\": \"\",\r\n\t\t\"birth_date\": \"\",\r\n\t\t\"grade\": \"10\",\r\n\t\t\"emp_type\": \"2\",\r\n\t\t\"position_id\": \"60042316\",\r\n\t\t\"position_name\": \"ผู้รักษาเงิน\",\r\n\t\t\"email\": \"THUNYANAN.U@GHB.CO.TH\",\r\n\t\t\"tel\": \"\",\r\n\t\t\"org_level\": \"601\",\r\n\t\t\"comp_objid\": \"50000001\",\r\n\t\t\"comp_code\": \"D9100\",\r\n\t\t\"comp_short_name\": \"ธอส.\",\r\n\t\t\"comp_name\": \"ธนาคารอาคารสงเคราะห์\",\r\n\t\t\"group_objid\": \"50004660\",\r\n\t\t\"group_code\": \"D9100\",\r\n\t\t\"group_short_name\": \"กง.นคร.\",\r\n\t\t\"group_name\": \"กลุ่มงานสาขา\",\r\n\t\t\"field_objid\": \"50004662\",\r\n\t\t\"field_code\": \"D9100\",\r\n\t\t\"field_short_name\": \"สง.นคร.\",\r\n\t\t\"field_name\": \"สายงานสาขานครหลวง\",\r\n\t\t\"dept_objid\": \"50004664\",\r\n\t\t\"dept_code\": \"88847\",\r\n\t\t\"dept_short_name\": \"สป.2\",\r\n\t\t\"dept_name\": \"ฝ่ายสาขา กทม.และปริมณฑล 2\",\r\n\t\t\"center_objid\": \"\",\r\n\t\t\"center_code\": \"\",\r\n\t\t\"center_short_name\": \"\",\r\n\t\t\"center_name\": \"\",\r\n\t\t\"zone_objid\": \"50005166\",\r\n\t\t\"zone_code\": \"77471\",\r\n\t\t\"zone_short_name\": \"สป.2(สนข.ต.)\",\r\n\t\t\"zone_name\": \"สำนักงานเขตกรุงเทพ-ใต้\",\r\n\t\t\"division_objid\": \"\",\r\n\t\t\"division_code\": \"\",\r\n\t\t\"division_short_name\": \"\",\r\n\t\t\"division_name\": \"\",\r\n\t\t\"sol_objid\": \"50005159\",\r\n\t\t\"sol_code\": \"2600\",\r\n\t\t\"sol_short_name\": \"สป.2(พร.2)\",\r\n\t\t\"sol_name\": \"สาขาพระราม 2\",\r\n\t\t\"sub_sol_objid\": \"\",\r\n\t\t\"sub_sol_code\": \"\",\r\n\t\t\"sub_sol_short_name\": \"\",\r\n\t\t\"sub_sol_name\": \"\",\r\n\t\t\"start_date\": \"1990-04-02T00:00:00\",\r\n\t\t\"stop_date\": \"9999-12-31T00:00:00\",\r\n\t\t\"last_action\": null,\r\n\t\t\"sol_instead_code\": \"1101\",\r\n\t\t\"sol_instead_name\": \"กรุงเทพมหานคร\",\r\n\t\t\"sol_hq\": \"1100\",\r\n\t\t\"plans_000\": \"0\",\r\n\t\t\"plansname_000\": \"\",\r\n\t\t\"org_000\": \"\",\r\n\t\t\"solid_000\": \"\",\r\n\t\t\"orgname_000\": \"\",\r\n\t\t\"org_748\": \"\",\r\n\t\t\"solid_748\": \"\",\r\n\t\t\"orgname_748\": \"\",\r\n\t\t\"create_date\": \"2024-09-05T15:45:20.457\",\r\n\t\t\"update_date\": \"2024-09-05T15:45:20.457\"\r\n\t}\r\n  ]\r\n}";
            //************** oAuth/Token ***************
            //using (var client = new HttpClient())
            //{
            //    try
            //    {
            //        var endPoint = new Uri(_iniCon.itUrl_v2);
            //        //var result = client.GetAsync(endPoint).Result;
            //        _logSys.WriteProcessLogFile(_strPathFile, "Uri(endPoint) : " + endPoint);
            //        //var json = result.Content.ReadAsStringAsync();
            //        //_logSys.WriteProcessLogFile(_strPathFile, "result.Content.ReadAsStringAsync() : " + json);

            //        var parameters = new List<KeyValuePair<string, string>>
            //        {
            //            new KeyValuePair<string, string>("grant_type", _iniCon.itldap3_v2),
            //            new KeyValuePair<string, string>("username", _iniCon.itldap1_v2),
            //            new KeyValuePair<string, string>("password", _iniCon.itldap2_v2)
            //        };

            //        // สร้าง FormUrlEncodedContent จากพารามิเตอร์
            //        HttpContent content = new FormUrlEncodedContent(parameters);
            //        _logSys.WriteProcessLogFile(_strPathFile, "FormUrlEncodedContent : " + parameters);
            //        // กำหนด Headers (ถ้ามี)
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            //        // ส่ง POST Request ไปยัง API
            //        //HttpResponseMessage response = await client.PostAsync(endPoint, content);
            //        var result1 = client.PostAsync(endPoint, content).Result.Content.ReadAsStringAsync().Result;
            //        _logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,content) : " + result1);
            //        tokendata = JsonConvert.DeserializeObject<dynamic>(result1);//.Content.ReadAsStringAsync();
            //        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + tokendata);
            //        //codeR.emp_code = "41830";
            //    }
            //    catch (Exception ex)
            //    {
            //        _logSys.WriteProcessLogFile(_strPathFile, "Error HttpRequestV2 : " + ex.Message.ToString());
            //    }
            //}

            //************** SignIn/Auth(User) ***************
            if (userReq.username != "" && userReq.password != "")
            {
                _logSys.WriteProcessLogFile(_strPathFile, "*** Begin verified user *** ");
                //var user_endPoint = new Uri(_iniCon.authenUrl_v2);
                _logSys.WriteProcessLogFile(_strPathFile, "user_endPoint : " + _iniCon.authenUrl);
                using (var client2 = new HttpClient())
                {
                    try
                    {
                        //var userRequest = new UserPostRequest
                        //{
                        //    username = userReq.username,
                        //    password = userReq.password
                        //};
                        //var user_newpostjson = JsonConvert.SerializeObject(userRequest);
                        ////_logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + user_newpostjson + " / " + tokendata.access_token);
                        //var user_playload = new StringContent(user_newpostjson, Encoding.UTF8, "application/json");
                        //_logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + user_playload + " / " + user_playload.Headers);
                        //string authorization = tokendata.access_token;
                        //// Set the authorization header with a Bearer token  
                        //client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
                        //_logSys.WriteProcessLogFile(_strPathFile, "authorization: " + tokendata.token_type + " " + tokendata.access_token);
                        ////client2.DefaultRequestHeaders.Add("X-Auth-Token", authorization);
                        //var result1 = client2.PostAsync(user_endPoint, user_playload).Result.Content.ReadAsStringAsync().Result;
                        //_logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,playload) : " + result1);
                        userdata = JsonConvert.DeserializeObject<dynamic>(result6);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + userdata);
                        var chkStu = userdata.status.ToString();
                        if (chkStu.Trim().ToLower() != "false")
                        {
                            var dt = userdata.data;

                            ldapUser.emp_code = dt[0].emp_code;
                            ldapUser.fname = dt[0].fname;
                            ldapUser.lname = dt[0].lname;
                            ldapUser.id_card = dt[0].id_card;
                            ldapUser.birth_date = dt[0].birth_date;
                            ldapUser.grade = dt[0].grade;
                            ldapUser.emp_type = dt[0].emp_type;
                            ldapUser.position_id = dt[0].position_id;
                            ldapUser.position_name = dt[0].position_name;
                            ldapUser.email = dt[0].email;
                            ldapUser.tel = dt[0].tel;
                            ldapUser.org_level = dt[0].org_level;
                            ldapUser.comp_objid = dt[0].comp_objid;
                            ldapUser.comp_code = dt[0].comp_code;
                            ldapUser.comp_short_name = dt[0].comp_short_name;
                            ldapUser.comp_name = dt[0].comp_name;
                            ldapUser.group_objid = dt[0].group_objid;
                            ldapUser.group_code = dt[0].group_code;
                            ldapUser.group_short_name = dt[0].group_short_name;
                            ldapUser.group_name = dt[0].group_name;
                            ldapUser.field_objid = dt[0].field_objid;
                            ldapUser.field_code = dt[0].field_code;
                            ldapUser.field_short_name = dt[0].field_short_name;
                            ldapUser.field_name = dt[0].field_name;
                            ldapUser.dept_objid = dt[0].dept_objid;
                            ldapUser.dept_code = dt[0].dept_code;
                            ldapUser.dept_short_name = dt[0].dept_short_name;
                            ldapUser.dept_name = dt[0].dept_name;
                            ldapUser.center_objid = dt[0].center_objid;
                            ldapUser.center_code = dt[0].center_code;
                            ldapUser.center_short_name = dt[0].center_short_name;
                            ldapUser.center_name = dt[0].center_name;
                            ldapUser.zone_objid = dt[0].zone_objid;
                            ldapUser.zone_code = dt[0].zone_code;
                            ldapUser.zone_short_name = dt[0].zone_short_names;
                            ldapUser.zone_name = dt[0].zone_name;
                            ldapUser.division_objid = dt[0].division_objid;
                            ldapUser.division_code = dt[0].division_code;
                            ldapUser.division_short_name = dt[0].division_short_name;
                            ldapUser.division_name = dt[0].division_name;
                            ldapUser.sol_objid = dt[0].sol_objid;
                            ldapUser.sol_code = dt[0].sol_code;
                            ldapUser.sol_short_name = dt[0].sol_short_name;
                            ldapUser.sol_name = dt[0].sol_name;
                            ldapUser.sub_sol_objid = dt[0].sub_sol_objid;
                            ldapUser.sub_sol_code = dt[0].sub_sol_code;
                            ldapUser.sub_sol_short_name = dt[0].sub_sol_short_name;
                            ldapUser.sub_sol_name = dt[0].sub_sol_name;
                            ldapUser.start_date = dt[0].start_date;
                            ldapUser.stop_date = dt[0].stop_date;
                            ldapUser.last_action = dt[0].last_action;
                            ldapUser.sol_instead_code = dt[0].sol_instead_code;
                            ldapUser.sol_instead_name = dt[0].sol_instead_name;
                            ldapUser.sol_hq = dt[0].sol_hq;
                            ldapUser.plans_000 = dt[0].plans_000;
                            ldapUser.plansname_000 = dt[0].plansname_000;
                            ldapUser.org_000 = dt[0].org_000;
                            ldapUser.solid_000 = dt[0].solid_000;
                            ldapUser.orgname_000 = dt[0].orgname_000;
                            ldapUser.org_748 = dt[0].org_748;
                            ldapUser.solid_748 = dt[0].solid_748;
                            ldapUser.orgname_748 = dt[0].orgname_748;
                            ldapUser.create_date = dt[0].create_date;
                            ldapUser.update_date = dt[0].update_date;
                            _logSys.WriteProcessLogFile(_strPathFile, "add User Details  " + ldapUser.fname);
                            _logSys.WriteProcessLogFile(_strPathFile, "Show Details userdata.empCode : " + userdata.emp_Code + " / userdata.lName : " + userdata.lName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "This is not User ldap.  ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
                    }
                }
            }

            //************** Emp/Code ***************
            /*
            if (null != tokendata && codeR.emp_code != "")
            {
                _logSys.WriteProcessLogFile(_strPathFile, "*** Begin verified with code *** ");
                var user_endPoint = new Uri(_iniCon.codeUrl_v2);
                _logSys.WriteProcessLogFile(_strPathFile, "user_endPoint : " + _iniCon.codeUrl_v2);
                using (var client2 = new HttpClient())
                {
                    try
                    {
                        var userCodeRequest = new UserPostCodeRequest
                        {
                            emp_code = codeR.emp_code
                        };
                        var user_newpostjson = JsonConvert.SerializeObject(userCodeRequest);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + user_newpostjson + " / " + tokendata.access_token);
                        var user_playload = new StringContent(user_newpostjson, Encoding.UTF8, "application/json");
                        _logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + user_playload + " / " + user_playload.Headers);
                        string authorization = tokendata.access_token;
                        // Set the authorization header with a Bearer token  
                        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
                        _logSys.WriteProcessLogFile(_strPathFile, "authorization: " + tokendata.token_type + " " + tokendata.access_token);
                        //client2.DefaultRequestHeaders.Add("X-Auth-Token", authorization);
                        var result1 = client2.PostAsync(user_endPoint, user_playload).Result.Content.ReadAsStringAsync().Result;
                        _logSys.WriteProcessLogFile(_strPathFile, "client2.PostAsync(user_endPoint,user_playload) : " + result1);
                        userdata = JsonConvert.DeserializeObject<dynamic>(result1);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + userdata);
                        if (null != userdata)
                        {
                            ldapUser.emp_code = userdata.emp_code;
                            ldapUser.fname = userdata.fname;
                            ldapUser.lname = userdata.lname;
                            ldapUser.id_card = userdata.id_card;
                            ldapUser.birth_date = userdata.birth_date;
                            ldapUser.grade = userdata.grade;
                            ldapUser.emp_type = userdata.emp_type;
                            ldapUser.position_id = userdata.position_id;
                            ldapUser.position_name = userdata.position_name;
                            ldapUser.email = userdata.email;
                            ldapUser.tel = userdata.tel;
                            ldapUser.org_level = userdata.org_level;
                            ldapUser.comp_objid = userdata.comp_objid;
                            ldapUser.comp_code = userdata.comp_code;
                            ldapUser.comp_short_name = userdata.comp_short_name;
                            ldapUser.comp_name = userdata.comp_name;
                            ldapUser.group_objid = userdata.group_objid;
                            ldapUser.group_code = userdata.group_code;
                            ldapUser.group_short_name = userdata.group_short_name;
                            ldapUser.group_name = userdata.group_name;
                            ldapUser.field_objid = userdata.field_objid;
                            ldapUser.field_code = userdata.field_code;
                            ldapUser.field_short_name = userdata.field_short_name;
                            ldapUser.field_name = userdata.field_name;
                            ldapUser.dept_objid = userdata.dept_objid;
                            ldapUser.dept_code = userdata.dept_code;
                            ldapUser.dept_short_name = userdata.dept_short_name;
                            ldapUser.dept_name = userdata.dept_name;
                            ldapUser.center_objid = userdata.center_objid;
                            ldapUser.center_code = userdata.center_code;
                            ldapUser.center_short_name = userdata.center_short_name;
                            ldapUser.center_name = userdata.center_name;
                            ldapUser.zone_objid = userdata.zone_objid;
                            ldapUser.zone_code = userdata.zone_code;
                            ldapUser.zone_short_name = userdata.zone_short_names;
                            ldapUser.zone_name = userdata.zone_name;
                            ldapUser.division_objid = userdata.division_objid;
                            ldapUser.division_code = userdata.division_code;
                            ldapUser.division_short_name = userdata.division_short_name;
                            ldapUser.division_name = userdata.division_name;
                            ldapUser.sol_objid = userdata.sol_objid;
                            ldapUser.sol_code = userdata.sol_code;
                            ldapUser.sol_short_name = userdata.sol_short_name;
                            ldapUser.sol_name = userdata.sol_name;
                            ldapUser.sub_sol_objid = userdata.sub_sol_objid;
                            ldapUser.sub_sol_code = userdata.sub_sol_code;
                            ldapUser.sub_sol_short_name = userdata.sub_sol_short_name;
                            ldapUser.sub_sol_name = userdata.sub_sol_name;
                            ldapUser.start_date = userdata.start_date;
                            ldapUser.stop_date = userdata.stop_date;
                            ldapUser.last_action = userdata.last_action;
                            ldapUser.sol_instead_code = userdata.sol_instead_code;
                            ldapUser.sol_instead_name = userdata.sol_instead_name;
                            ldapUser.sol_hq = userdata.sol_hq;
                            ldapUser.plans_000 = userdata.plans_000;
                            ldapUser.plansname_000 = userdata.plansname_000;
                            ldapUser.org_000 = userdata.org_000;
                            ldapUser.solid_000 = userdata.solid_000;
                            ldapUser.orgname_000 = userdata.orgname_000;
                            ldapUser.org_748 = userdata.org_748;
                            ldapUser.solid_748 = userdata.solid_748;
                            ldapUser.orgname_748 = userdata.orgname_748;
                            ldapUser.create_date = userdata.create_date;
                            ldapUser.update_date = userdata.update_date;
                            _logSys.WriteProcessLogFile(_strPathFile, "add User Details  " + ldapUser.fname);
                            _logSys.WriteProcessLogFile(_strPathFile, "Show Details userdata.empCode : " + userdata.emp_Code + " / userdata.lName : " + userdata.lName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "This is not User ldap.  ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
                    }
                }
            }
            */
            return ldapUser;
        }
        //Vesion3 Emp/Code
        public ApiPostResponse2 HttpRequestV3(UserPostRequest userReq, String empCode)
        {
            ApiPostResponse2 ldapUser = new ApiPostResponse2();
            _logSys.WriteProcessLogFile(_strPathFile, "<<< Start Sup HttpRequestV3 Emp/Code >>> ");
            _logSys.WriteProcessLogFile(_strPathFile, "Get Token Step ");
            dynamic tokendata = null;
            dynamic userdata = null;
            UserPostCodeRequest codeR = new UserPostCodeRequest();
            codeR.emp_code = empCode;

            #region oAuth/Token
            //************** oAuth/Token ***************

            using (var client = new HttpClient())
            {
                try
                {
                    var endPoint = new Uri(_iniCon.itUrl_v2);
                    //var result = client.GetAsync(endPoint).Result;
                    _logSys.WriteProcessLogFile(_strPathFile, "Uri(endPoint) : " + endPoint);
                    //var json = result.Content.ReadAsStringAsync();
                    //_logSys.WriteProcessLogFile(_strPathFile, "result.Content.ReadAsStringAsync() : " + json);

                    var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", _iniCon.itldap3_v2),
                        new KeyValuePair<string, string>("username", _iniCon.itldap1_v2),
                        new KeyValuePair<string, string>("password", _iniCon.itldap2_v2)
                    };

                    // สร้าง FormUrlEncodedContent จากพารามิเตอร์
                    HttpContent content = new FormUrlEncodedContent(parameters);
                    _logSys.WriteProcessLogFile(_strPathFile, "FormUrlEncodedContent : " + parameters);
                    // กำหนด Headers (ถ้ามี)
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                    // ส่ง POST Request ไปยัง API
                    //HttpResponseMessage response = await client.PostAsync(endPoint, content);
                    var result1 = client.PostAsync(endPoint, content).Result.Content.ReadAsStringAsync().Result;
                    _logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,content) : " + result1);
                    tokendata = JsonConvert.DeserializeObject<dynamic>(result1);//.Content.ReadAsStringAsync();
                    _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + tokendata);
                    //codeR.emp_code = "41830";
                }
                catch (Exception ex)
                {
                    _logSys.WriteProcessLogFile(_strPathFile, "Error HttpRequestV2 : " + ex.Message.ToString());
                }
            }

            #endregion oAuth/Token

            #region Auth(User)
            //************** SignIn/Auth(User) ***************
            /*
            if (null != tokendata && userReq.username != "" && userReq.password != "")
            {
                _logSys.WriteProcessLogFile(_strPathFile, "*** Begin verified user *** ");
                var user_endPoint = new Uri(_iniCon.authenUrl_v2);
                _logSys.WriteProcessLogFile(_strPathFile, "user_endPoint : " + _iniCon.authenUrl);
                using (var client2 = new HttpClient())
                {
                    try
                    {
                        var userRequest = new UserPostRequest
                        {
                            username = userReq.username,
                            password = userReq.password
                        };
                        var user_newpostjson = JsonConvert.SerializeObject(userRequest);
                        //_logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + user_newpostjson + " / " + tokendata.access_token);
                        var user_playload = new StringContent(user_newpostjson, Encoding.UTF8, "application/json");
                        _logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + user_playload + " / " + user_playload.Headers);
                        string authorization = tokendata.access_token;
                        // Set the authorization header with a Bearer token  
                        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
                        _logSys.WriteProcessLogFile(_strPathFile, "authorization: " + tokendata.token_type + " " + tokendata.access_token);
                        //client2.DefaultRequestHeaders.Add("X-Auth-Token", authorization);
                        var result1 = client2.PostAsync(user_endPoint, user_playload).Result.Content.ReadAsStringAsync().Result;
                        _logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,playload) : " + result1);
                        userdata = JsonConvert.DeserializeObject<dynamic>(result1);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + userdata);
                        var chkStu = userdata.status.ToString();
                        if (chkStu.Trim().ToLower() != "false")
                        {
                            var dt = userdata.data;

                            ldapUser.emp_code = dt[0].emp_code;
                            ldapUser.fname = dt[0].fname;
                            ldapUser.lname = dt[0].lname;
                            ldapUser.id_card = dt[0].id_card;
                            ldapUser.birth_date = dt[0].birth_date;
                            ldapUser.grade = dt[0].grade;
                            ldapUser.emp_type = dt[0].emp_type;
                            ldapUser.position_id = dt[0].position_id;
                            ldapUser.position_name = dt[0].position_name;
                            ldapUser.email = dt[0].email;
                            ldapUser.tel = dt[0].tel;
                            ldapUser.org_level = dt[0].org_level;
                            ldapUser.comp_objid = dt[0].comp_objid;
                            ldapUser.comp_code = dt[0].comp_code;
                            ldapUser.comp_short_name = dt[0].comp_short_name;
                            ldapUser.comp_name = dt[0].comp_name;
                            ldapUser.group_objid = dt[0].group_objid;
                            ldapUser.group_code = dt[0].group_code;
                            ldapUser.group_short_name = dt[0].group_short_name;
                            ldapUser.group_name = dt[0].group_name;
                            ldapUser.field_objid = dt[0].field_objid;
                            ldapUser.field_code = dt[0].field_code;
                            ldapUser.field_short_name = dt[0].field_short_name;
                            ldapUser.field_name = dt[0].field_name;
                            ldapUser.dept_objid = dt[0].dept_objid;
                            ldapUser.dept_code = dt[0].dept_code;
                            ldapUser.dept_short_name = dt[0].dept_short_name;
                            ldapUser.dept_name = dt[0].dept_name;
                            ldapUser.center_objid = dt[0].center_objid;
                            ldapUser.center_code = dt[0].center_code;
                            ldapUser.center_short_name = dt[0].center_short_name;
                            ldapUser.center_name = dt[0].center_name;
                            ldapUser.zone_objid = dt[0].zone_objid;
                            ldapUser.zone_code = dt[0].zone_code;
                            ldapUser.zone_short_name = dt[0].zone_short_names;
                            ldapUser.zone_name = dt[0].zone_name;
                            ldapUser.division_objid = dt[0].division_objid;
                            ldapUser.division_code = dt[0].division_code;
                            ldapUser.division_short_name = dt[0].division_short_name;
                            ldapUser.division_name = dt[0].division_name;
                            ldapUser.sol_objid = dt[0].sol_objid;
                            ldapUser.sol_code = dt[0].sol_code;
                            ldapUser.sol_short_name = dt[0].sol_short_name;
                            ldapUser.sol_name = dt[0].sol_name;
                            ldapUser.sub_sol_objid = dt[0].sub_sol_objid;
                            ldapUser.sub_sol_code = dt[0].sub_sol_code;
                            ldapUser.sub_sol_short_name = dt[0].sub_sol_short_name;
                            ldapUser.sub_sol_name = dt[0].sub_sol_name;
                            ldapUser.start_date = dt[0].start_date;
                            ldapUser.stop_date = dt[0].stop_date;
                            ldapUser.last_action = dt[0].last_action;
                            ldapUser.sol_instead_code = dt[0].sol_instead_code;
                            ldapUser.sol_instead_name = dt[0].sol_instead_name;
                            ldapUser.sol_hq = dt[0].sol_hq;
                            ldapUser.plans_000 = dt[0].plans_000;
                            ldapUser.plansname_000 = dt[0].plansname_000;
                            ldapUser.org_000 = dt[0].org_000;
                            ldapUser.solid_000 = dt[0].solid_000;
                            ldapUser.orgname_000 = dt[0].orgname_000;
                            ldapUser.org_748 = dt[0].org_748;
                            ldapUser.solid_748 = dt[0].solid_748;
                            ldapUser.orgname_748 = dt[0].orgname_748;
                            ldapUser.create_date = dt[0].create_date;
                            ldapUser.update_date = dt[0].update_date;
                            _logSys.WriteProcessLogFile(_strPathFile, "add User Details  " + ldapUser.fname);
                            _logSys.WriteProcessLogFile(_strPathFile, "Show Details userdata.empCode : " + userdata.emp_Code + " / userdata.lName : " + userdata.lName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "This is not User ldap.  ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
                    }
                }
            }
            */
            #endregion Auth(User)

            //************** Emp/Code ***************

            if (codeR.emp_code != "")
            {
                _logSys.WriteProcessLogFile(_strPathFile, "*** Begin verified with code *** ");
                var user_endPoint = new Uri(_iniCon.codeUrl_v2);
                _logSys.WriteProcessLogFile(_strPathFile, "user_endPoint : " + _iniCon.codeUrl_v2);
                using (var client2 = new HttpClient())
                {
                    try
                    {

                        var userCodeRequest = new UserPostCodeRequest
                        {
                            emp_code = codeR.emp_code
                        };
                        var user_newpostjson = JsonConvert.SerializeObject(userCodeRequest);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + user_newpostjson + " / " + tokendata.access_token);
                        var user_playload = new StringContent(user_newpostjson, Encoding.UTF8, "application/json");
                        _logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + user_playload + " / " + user_playload.Headers);
                        string authorization = tokendata.access_token;
                        // Set the authorization header with a Bearer token  
                        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
                        _logSys.WriteProcessLogFile(_strPathFile, "authorization: " + tokendata.token_type + " " + tokendata.access_token);
                        //client2.DefaultRequestHeaders.Add("X-Auth-Token", authorization);
                        var result1 = client2.PostAsync(user_endPoint, user_playload).Result.Content.ReadAsStringAsync().Result;
                        _logSys.WriteProcessLogFile(_strPathFile, "client2.PostAsync(user_endPoint,user_playload) : " + result1);
                        userdata = JsonConvert.DeserializeObject<dynamic>(result1);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + userdata);
                        var chkStu = userdata.status.ToString();
                        if (chkStu.Trim().ToLower() != "false")
                        {
                            var dt = userdata.data;

                            ldapUser.emp_code = dt[0].emp_code;
                            ldapUser.fname = dt[0].fname;
                            ldapUser.lname = dt[0].lname;
                            ldapUser.id_card = dt[0].id_card;
                            ldapUser.birth_date = dt[0].birth_date;
                            ldapUser.grade = dt[0].grade;
                            ldapUser.emp_type = dt[0].emp_type;
                            ldapUser.position_id = dt[0].position_id;
                            ldapUser.position_name = dt[0].position_name;
                            ldapUser.email = dt[0].email;
                            ldapUser.tel = dt[0].tel;
                            ldapUser.org_level = dt[0].org_level;
                            ldapUser.comp_objid = dt[0].comp_objid;
                            ldapUser.comp_code = dt[0].comp_code;
                            ldapUser.comp_short_name = dt[0].comp_short_name;
                            ldapUser.comp_name = dt[0].comp_name;
                            ldapUser.group_objid = dt[0].group_objid;
                            ldapUser.group_code = dt[0].group_code;
                            ldapUser.group_short_name = dt[0].group_short_name;
                            ldapUser.group_name = dt[0].group_name;
                            ldapUser.field_objid = dt[0].field_objid;
                            ldapUser.field_code = dt[0].field_code;
                            ldapUser.field_short_name = dt[0].field_short_name;
                            ldapUser.field_name = dt[0].field_name;
                            ldapUser.dept_objid = dt[0].dept_objid;
                            ldapUser.dept_code = dt[0].dept_code;
                            ldapUser.dept_short_name = dt[0].dept_short_name;
                            ldapUser.dept_name = dt[0].dept_name;
                            ldapUser.center_objid = dt[0].center_objid;
                            ldapUser.center_code = dt[0].center_code;
                            ldapUser.center_short_name = dt[0].center_short_name;
                            ldapUser.center_name = dt[0].center_name;
                            ldapUser.zone_objid = dt[0].zone_objid;
                            ldapUser.zone_code = dt[0].zone_code;
                            ldapUser.zone_short_name = dt[0].zone_short_names;
                            ldapUser.zone_name = dt[0].zone_name;
                            ldapUser.division_objid = dt[0].division_objid;
                            ldapUser.division_code = dt[0].division_code;
                            ldapUser.division_short_name = dt[0].division_short_name;
                            ldapUser.division_name = dt[0].division_name;
                            ldapUser.sol_objid = dt[0].sol_objid;
                            ldapUser.sol_code = dt[0].sol_code;
                            ldapUser.sol_short_name = dt[0].sol_short_name;
                            ldapUser.sol_name = dt[0].sol_name;
                            ldapUser.sub_sol_objid = dt[0].sub_sol_objid;
                            ldapUser.sub_sol_code = dt[0].sub_sol_code;
                            ldapUser.sub_sol_short_name = dt[0].sub_sol_short_name;
                            ldapUser.sub_sol_name = dt[0].sub_sol_name;
                            ldapUser.start_date = dt[0].start_date;
                            ldapUser.stop_date = dt[0].stop_date;
                            ldapUser.last_action = dt[0].last_action;
                            ldapUser.sol_instead_code = dt[0].sol_instead_code;
                            ldapUser.sol_instead_name = dt[0].sol_instead_name;
                            ldapUser.sol_hq = dt[0].sol_hq;
                            ldapUser.plans_000 = dt[0].plans_000;
                            ldapUser.plansname_000 = dt[0].plansname_000;
                            ldapUser.org_000 = dt[0].org_000;
                            ldapUser.solid_000 = dt[0].solid_000;
                            ldapUser.orgname_000 = dt[0].orgname_000;
                            ldapUser.org_748 = dt[0].org_748;
                            ldapUser.solid_748 = dt[0].solid_748;
                            ldapUser.orgname_748 = dt[0].orgname_748;
                            ldapUser.create_date = dt[0].create_date;
                            ldapUser.update_date = dt[0].update_date;
                            _logSys.WriteProcessLogFile(_strPathFile, "add User Details  " + ldapUser.fname);
                            _logSys.WriteProcessLogFile(_strPathFile, "Show Details userdata.empCode : " + userdata.emp_Code + " / userdata.lName : " + userdata.lName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "This is not User ldap.  ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
                    }
                }
            }

            return ldapUser;
        }

        public ApiPostResponse2 HttpRequestV3_2(UserPostRequest userReq, String empCode)
        {
            ApiPostResponse2 ldapUser = new ApiPostResponse2();
            _logSys.WriteProcessLogFile(_strPathFile, "<<< Start Sup HttpRequestV3 Emp/Code >>> ");
            _logSys.WriteProcessLogFile(_strPathFile, "Get Token Step ");
            dynamic tokendata = null;
            dynamic userdata = null;
            UserPostCodeRequest codeR = new UserPostCodeRequest();
            codeR.emp_code = empCode;

            #region oAuth/Token
            //************** oAuth/Token ***************

            using (var client = new HttpClient())
            {
                try
                {
                    var endPoint = new Uri(_iniCon.itUrl_v2);
                    //var result = client.GetAsync(endPoint).Result;
                    _logSys.WriteProcessLogFile(_strPathFile, "Uri(endPoint) : " + endPoint);
                    //var json = result.Content.ReadAsStringAsync();
                    //_logSys.WriteProcessLogFile(_strPathFile, "result.Content.ReadAsStringAsync() : " + json);

                    var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", _iniCon.itldap3_v2),
                        new KeyValuePair<string, string>("username", _iniCon.itldap1_v2),
                        new KeyValuePair<string, string>("password", _iniCon.itldap2_v2)
                    };

                    // สร้าง FormUrlEncodedContent จากพารามิเตอร์
                    HttpContent content = new FormUrlEncodedContent(parameters);
                    _logSys.WriteProcessLogFile(_strPathFile, "FormUrlEncodedContent : " + parameters);
                    // กำหนด Headers (ถ้ามี)
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                    // ส่ง POST Request ไปยัง API
                    //HttpResponseMessage response = await client.PostAsync(endPoint, content);
                    var result1 = client.PostAsync(endPoint, content).Result.Content.ReadAsStringAsync().Result;
                    _logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,content) : " + result1);
                    tokendata = JsonConvert.DeserializeObject<dynamic>(result1);//.Content.ReadAsStringAsync();
                    _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + tokendata);
                    //codeR.emp_code = "41830";
                }
                catch (Exception ex)
                {
                    _logSys.WriteProcessLogFile(_strPathFile, "Error HttpRequestV2 : " + ex.Message.ToString());
                }
            }

            #endregion oAuth/Token

            #region Auth(User)
            //************** SignIn/Auth(User) ***************
            /*
            if (null != tokendata && userReq.username != "" && userReq.password != "")
            {
                _logSys.WriteProcessLogFile(_strPathFile, "*** Begin verified user *** ");
                var user_endPoint = new Uri(_iniCon.authenUrl_v2);
                _logSys.WriteProcessLogFile(_strPathFile, "user_endPoint : " + _iniCon.authenUrl);
                using (var client2 = new HttpClient())
                {
                    try
                    {
                        var userRequest = new UserPostRequest
                        {
                            username = userReq.username,
                            password = userReq.password
                        };
                        var user_newpostjson = JsonConvert.SerializeObject(userRequest);
                        //_logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + user_newpostjson + " / " + tokendata.access_token);
                        var user_playload = new StringContent(user_newpostjson, Encoding.UTF8, "application/json");
                        _logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + user_playload + " / " + user_playload.Headers);
                        string authorization = tokendata.access_token;
                        // Set the authorization header with a Bearer token  
                        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
                        _logSys.WriteProcessLogFile(_strPathFile, "authorization: " + tokendata.token_type + " " + tokendata.access_token);
                        //client2.DefaultRequestHeaders.Add("X-Auth-Token", authorization);
                        var result1 = client2.PostAsync(user_endPoint, user_playload).Result.Content.ReadAsStringAsync().Result;
                        _logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,playload) : " + result1);
                        userdata = JsonConvert.DeserializeObject<dynamic>(result1);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + userdata);
                        var chkStu = userdata.status.ToString();
                        if (chkStu.Trim().ToLower() != "false")
                        {
                            var dt = userdata.data;

                            ldapUser.emp_code = dt[0].emp_code;
                            ldapUser.fname = dt[0].fname;
                            ldapUser.lname = dt[0].lname;
                            ldapUser.id_card = dt[0].id_card;
                            ldapUser.birth_date = dt[0].birth_date;
                            ldapUser.grade = dt[0].grade;
                            ldapUser.emp_type = dt[0].emp_type;
                            ldapUser.position_id = dt[0].position_id;
                            ldapUser.position_name = dt[0].position_name;
                            ldapUser.email = dt[0].email;
                            ldapUser.tel = dt[0].tel;
                            ldapUser.org_level = dt[0].org_level;
                            ldapUser.comp_objid = dt[0].comp_objid;
                            ldapUser.comp_code = dt[0].comp_code;
                            ldapUser.comp_short_name = dt[0].comp_short_name;
                            ldapUser.comp_name = dt[0].comp_name;
                            ldapUser.group_objid = dt[0].group_objid;
                            ldapUser.group_code = dt[0].group_code;
                            ldapUser.group_short_name = dt[0].group_short_name;
                            ldapUser.group_name = dt[0].group_name;
                            ldapUser.field_objid = dt[0].field_objid;
                            ldapUser.field_code = dt[0].field_code;
                            ldapUser.field_short_name = dt[0].field_short_name;
                            ldapUser.field_name = dt[0].field_name;
                            ldapUser.dept_objid = dt[0].dept_objid;
                            ldapUser.dept_code = dt[0].dept_code;
                            ldapUser.dept_short_name = dt[0].dept_short_name;
                            ldapUser.dept_name = dt[0].dept_name;
                            ldapUser.center_objid = dt[0].center_objid;
                            ldapUser.center_code = dt[0].center_code;
                            ldapUser.center_short_name = dt[0].center_short_name;
                            ldapUser.center_name = dt[0].center_name;
                            ldapUser.zone_objid = dt[0].zone_objid;
                            ldapUser.zone_code = dt[0].zone_code;
                            ldapUser.zone_short_name = dt[0].zone_short_names;
                            ldapUser.zone_name = dt[0].zone_name;
                            ldapUser.division_objid = dt[0].division_objid;
                            ldapUser.division_code = dt[0].division_code;
                            ldapUser.division_short_name = dt[0].division_short_name;
                            ldapUser.division_name = dt[0].division_name;
                            ldapUser.sol_objid = dt[0].sol_objid;
                            ldapUser.sol_code = dt[0].sol_code;
                            ldapUser.sol_short_name = dt[0].sol_short_name;
                            ldapUser.sol_name = dt[0].sol_name;
                            ldapUser.sub_sol_objid = dt[0].sub_sol_objid;
                            ldapUser.sub_sol_code = dt[0].sub_sol_code;
                            ldapUser.sub_sol_short_name = dt[0].sub_sol_short_name;
                            ldapUser.sub_sol_name = dt[0].sub_sol_name;
                            ldapUser.start_date = dt[0].start_date;
                            ldapUser.stop_date = dt[0].stop_date;
                            ldapUser.last_action = dt[0].last_action;
                            ldapUser.sol_instead_code = dt[0].sol_instead_code;
                            ldapUser.sol_instead_name = dt[0].sol_instead_name;
                            ldapUser.sol_hq = dt[0].sol_hq;
                            ldapUser.plans_000 = dt[0].plans_000;
                            ldapUser.plansname_000 = dt[0].plansname_000;
                            ldapUser.org_000 = dt[0].org_000;
                            ldapUser.solid_000 = dt[0].solid_000;
                            ldapUser.orgname_000 = dt[0].orgname_000;
                            ldapUser.org_748 = dt[0].org_748;
                            ldapUser.solid_748 = dt[0].solid_748;
                            ldapUser.orgname_748 = dt[0].orgname_748;
                            ldapUser.create_date = dt[0].create_date;
                            ldapUser.update_date = dt[0].update_date;
                            _logSys.WriteProcessLogFile(_strPathFile, "add User Details  " + ldapUser.fname);
                            _logSys.WriteProcessLogFile(_strPathFile, "Show Details userdata.empCode : " + userdata.emp_Code + " / userdata.lName : " + userdata.lName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "This is not User ldap.  ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
                    }
                }
            }
            */
            #endregion Auth(User)

            //************** Emp/Code ***************

            if (codeR.emp_code != "")
            {
                _logSys.WriteProcessLogFile(_strPathFile, "*** Begin verified with code *** ");
                var user_endPoint = new Uri(_iniCon.codeUrl_v2);
                _logSys.WriteProcessLogFile(_strPathFile, "user_endPoint : " + _iniCon.codeUrl_v2);
                using (var client2 = new HttpClient())
                {
                    try
                    {

                        var userCodeRequest = new UserPostCodeRequest
                        {
                            emp_code = codeR.emp_code
                        };
                        var user_newpostjson = JsonConvert.SerializeObject(userCodeRequest);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + user_newpostjson + " / " + tokendata.access_token);
                        var user_playload = new StringContent(user_newpostjson, Encoding.UTF8, "application/json");
                        _logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + user_playload + " / " + user_playload.Headers);
                        string authorization = tokendata.access_token;
                        // Set the authorization header with a Bearer token  
                        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
                        _logSys.WriteProcessLogFile(_strPathFile, "authorization: " + tokendata.token_type + " " + tokendata.access_token);
                        //client2.DefaultRequestHeaders.Add("X-Auth-Token", authorization);
                        var result1 = client2.PostAsync(user_endPoint, user_playload).Result.Content.ReadAsStringAsync().Result;
                        _logSys.WriteProcessLogFile(_strPathFile, "client2.PostAsync(user_endPoint,user_playload) : " + result1);
                        userdata = JsonConvert.DeserializeObject<dynamic>(result1);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + userdata);
                        var chkStu = userdata.status.ToString();
                        if (chkStu.Trim().ToLower() != "false")
                        {
                            var dt = userdata.data;

                            ldapUser.emp_code = dt[0].emp_code;
                            ldapUser.fname = dt[0].fname;
                            ldapUser.lname = dt[0].lname;
                            ldapUser.id_card = dt[0].id_card;
                            ldapUser.birth_date = dt[0].birth_date;
                            ldapUser.grade = dt[0].grade;
                            ldapUser.emp_type = dt[0].emp_type;
                            ldapUser.position_id = dt[0].position_id;
                            ldapUser.position_name = dt[0].position_name;
                            ldapUser.email = dt[0].email;
                            ldapUser.tel = dt[0].tel;
                            ldapUser.org_level = dt[0].org_level;
                            ldapUser.comp_objid = dt[0].comp_objid;
                            ldapUser.comp_code = dt[0].comp_code;
                            ldapUser.comp_short_name = dt[0].comp_short_name;
                            ldapUser.comp_name = dt[0].comp_name;
                            ldapUser.group_objid = dt[0].group_objid;
                            ldapUser.group_code = dt[0].group_code;
                            ldapUser.group_short_name = dt[0].group_short_name;
                            ldapUser.group_name = dt[0].group_name;
                            ldapUser.field_objid = dt[0].field_objid;
                            ldapUser.field_code = dt[0].field_code;
                            ldapUser.field_short_name = dt[0].field_short_name;
                            ldapUser.field_name = dt[0].field_name;
                            ldapUser.dept_objid = dt[0].dept_objid;
                            ldapUser.dept_code = dt[0].dept_code;
                            ldapUser.dept_short_name = dt[0].dept_short_name;
                            ldapUser.dept_name = dt[0].dept_name;
                            ldapUser.center_objid = dt[0].center_objid;
                            ldapUser.center_code = dt[0].center_code;
                            ldapUser.center_short_name = dt[0].center_short_name;
                            ldapUser.center_name = dt[0].center_name;
                            ldapUser.zone_objid = dt[0].zone_objid;
                            ldapUser.zone_code = dt[0].zone_code;
                            ldapUser.zone_short_name = dt[0].zone_short_names;
                            ldapUser.zone_name = dt[0].zone_name;
                            ldapUser.division_objid = dt[0].division_objid;
                            ldapUser.division_code = dt[0].division_code;
                            ldapUser.division_short_name = dt[0].division_short_name;
                            ldapUser.division_name = dt[0].division_name;
                            ldapUser.sol_objid = dt[0].sol_objid;
                            ldapUser.sol_code = dt[0].sol_code;
                            ldapUser.sol_short_name = dt[0].sol_short_name;
                            ldapUser.sol_name = dt[0].sol_name;
                            ldapUser.sub_sol_objid = dt[0].sub_sol_objid;
                            ldapUser.sub_sol_code = dt[0].sub_sol_code;
                            ldapUser.sub_sol_short_name = dt[0].sub_sol_short_name;
                            ldapUser.sub_sol_name = dt[0].sub_sol_name;
                            ldapUser.start_date = dt[0].start_date;
                            ldapUser.stop_date = dt[0].stop_date;
                            ldapUser.last_action = dt[0].last_action;
                            ldapUser.sol_instead_code = dt[0].sol_instead_code;
                            ldapUser.sol_instead_name = dt[0].sol_instead_name;
                            ldapUser.sol_hq = dt[0].sol_hq;
                            ldapUser.plans_000 = dt[0].plans_000;
                            ldapUser.plansname_000 = dt[0].plansname_000;
                            ldapUser.org_000 = dt[0].org_000;
                            ldapUser.solid_000 = dt[0].solid_000;
                            ldapUser.orgname_000 = dt[0].orgname_000;
                            ldapUser.org_748 = dt[0].org_748;
                            ldapUser.solid_748 = dt[0].solid_748;
                            ldapUser.orgname_748 = dt[0].orgname_748;
                            ldapUser.create_date = dt[0].create_date;
                            ldapUser.update_date = dt[0].update_date;
                            _logSys.WriteProcessLogFile(_strPathFile, "add User Details  " + ldapUser.fname);
                            _logSys.WriteProcessLogFile(_strPathFile, "Show Details userdata.empCode : " + userdata.emp_Code + " / userdata.lName : " + userdata.lName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "This is not User ldap.  ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
                    }
                }
            }

            return ldapUser;
        }

        //Vesion4 Mockup user
        public ApiPostResponse2 HttpRequestV4(UserPostRequest userReq, String empCode)//20240913
        {
            ApiPostResponse2 ldapUser = new ApiPostResponse2();
            _logSys.WriteProcessLogFile(_strPathFile, "<<< Start Sup HttpRequestV2 >>> ");
            _logSys.WriteProcessLogFile(_strPathFile, "Get Token Step ");
            //dynamic tokendata = null;
            dynamic userdata = null;
            UserPostCodeRequest codeR = new UserPostCodeRequest();
            #region mock up

            string result1 = "{\r\n  \"status\": true,\r\n  \"message\": \"พบข้อมูลจำนวน 1 record\",\r\n  \"data\": [\r\n    {\r\n      \"emp_code\": \"50834\",\r\n      \"fname\": \"ชื่อพนักงาน 50834\",\r\n      \"lname\": \"นามสกุลพนักงาน 50834\",\r\n      \"id_card\": \"\",\r\n      \"birth_date\": \"\",\r\n      \"grade\": \"7\",\r\n      \"emp_type\": \"2\",\r\n      \"position_id\": \"60001469\",\r\n      \"position_name\": \"พนักงานคอมพิวเตอร์\",\r\n      \"email\": \"NATTAKIT.S@GHB.CO.TH\",\r\n      \"tel\": \"2128\",\r\n      \"org_level\": \"600\",\r\n      \"comp_objid\": \"50000001\",\r\n      \"comp_code\": \"D9100\",\r\n      \"comp_short_name\": \"ธอส.\",\r\n      \"comp_name\": \"ธนาคารอาคารสงเคราะห์\",\r\n      \"group_objid\": \"50005238\",\r\n      \"group_code\": \"D9100\",\r\n      \"group_short_name\": \"กง.ทส.\",\r\n      \"group_name\": \"กลุ่มงานเทคโนโลยีสารสนเทศ\",\r\n      \"field_objid\": \"50003795\",\r\n      \"field_code\": \"D9100\",\r\n      \"field_short_name\": \"สง.ทท.\",\r\n      \"field_name\": \"สายงานปฏิบัติการเทคโนโลยี\",\r\n      \"dept_objid\": \"50004538\",\r\n      \"dept_code\": \"88816\",\r\n      \"dept_short_name\": \"ปท.\",\r\n      \"dept_name\": \"ฝ่ายปฏิบัติการเทคโนโลยีสารสนเทศ\",\r\n      \"center_objid\": \"\",\r\n      \"center_code\": \"\",\r\n      \"center_short_name\": \"\",\r\n      \"center_name\": \"\",\r\n      \"zone_objid\": \"\",\r\n      \"zone_code\": \"\",\r\n      \"zone_short_name\": \"\",\r\n      \"zone_name\": \"\",\r\n      \"division_objid\": \"50004540\",\r\n      \"division_code\": \"88816\",\r\n      \"division_short_name\": \"ปท.(สจล.)\",\r\n      \"division_name\": \"ส่วนจัดการวิศวกรรมระบบงานหลัก\",\r\n      \"sol_objid\": \"\",\r\n      \"sol_code\": \"\",\r\n      \"sol_short_name\": \"\",\r\n      \"sol_name\": \"\",\r\n      \"sub_sol_objid\": \"\",\r\n      \"sub_sol_code\": \"\",\r\n      \"sub_sol_short_name\": \"\",\r\n      \"sub_sol_name\": \"\",\r\n      \"start_date\": \"2019-06-17T00:00:00\",\r\n      \"stop_date\": \"9999-12-31T00:00:00\",\r\n      \"last_action\": null,\r\n      \"sol_instead_code\": \"1001\",\r\n      \"sol_instead_name\": \"กรุงเทพมหานคร\",\r\n      \"sol_hq\": \"1000\",\r\n      \"plans_000\": \"0\",\r\n      \"plansname_000\": \"\",\r\n      \"org_000\": \"\",\r\n      \"solid_000\": \"\",\r\n      \"orgname_000\": \"\",\r\n      \"org_748\": \"\",\r\n      \"solid_748\": \"\",\r\n      \"orgname_748\": \"\",\r\n      \"create_date\": \"2024-09-05T15:45:20.457\",\r\n      \"update_date\": \"2024-09-05T15:45:20.457\"\r\n    }\r\n  ]\r\n}";

            string result2 = "{\r\n  \"status\": true,\r\n  \"message\": \"พบข้อมูลจำนวน 1 record\",\r\n  \"data\": [\r\n    {\r\n    \"emp_code\": \"02569\",\r\n    \"fname\": \"ชื่อพนักงาน 025689\",\r\n    \"lname\": \"นามสกุลพนักงาน 02568\",\r\n    \"id_card\": \"\",\r\n    \"birth_date\": \"\",\r\n    \"grade\": \"10\",\r\n    \"emp_type\": \"2\",\r\n    \"position_id\": \"60042316\",\r\n    \"position_name\": \"ผู้รักษาเงิน\",\r\n    \"email\": \"THUNYANAN9.U@GHB.CO.TH\",\r\n    \"tel\": \"\",\r\n    \"org_level\": \"601\",\r\n    \"comp_objid\": \"50000001\",\r\n    \"comp_code\": \"D9100\",\r\n    \"comp_short_name\": \"ธอส.\",\r\n    \"comp_name\": \"ธนาคารอาคารสงเคราะห์\",\r\n    \"group_objid\": \"50004660\",\r\n    \"group_code\": \"D9100\",\r\n    \"group_short_name\": \"กง.นคร.\",\r\n    \"group_name\": \"กลุ่มงานสาขา\",\r\n    \"field_objid\": \"50004662\",\r\n    \"field_code\": \"D9100\",\r\n    \"field_short_name\": \"สง.นคร.\",\r\n    \"field_name\": \"สายงานสาขานครหลวง\",\r\n    \"dept_objid\": \"50004664\",\r\n    \"dept_code\": \"88847\",\r\n    \"dept_short_name\": \"สป.2\",\r\n    \"dept_name\": \"ฝ่ายสาขา กทม.และปริมณฑล 2\",\r\n    \"center_objid\": \"\",\r\n    \"center_code\": \"\",\r\n    \"center_short_name\": \"\",\r\n    \"center_name\": \"\",\r\n    \"zone_objid\": \"50005166\",\r\n    \"zone_code\": \"77471\",\r\n    \"zone_short_name\": \"สป.2(สนข.ต.)\",\r\n    \"zone_name\": \"สำนักงานเขตกรุงเทพ-ใต้\",\r\n    \"division_objid\": \"\",\r\n    \"division_code\": \"\",\r\n    \"division_short_name\": \"\",\r\n    \"division_name\": \"\",\r\n    \"sol_objid\": \"50005159\",\r\n    \"sol_code\": \"2600\",\r\n    \"sol_short_name\": \"สป.2(พร.2)\",\r\n    \"sol_name\": \"สาขาพระราม 2\",\r\n    \"sub_sol_objid\": \"\",\r\n    \"sub_sol_code\": \"\",\r\n    \"sub_sol_short_name\": \"\",\r\n    \"sub_sol_name\": \"\",\r\n    \"start_date\": \"1990-04-02T00:00:00\",\r\n    \"stop_date\": \"9999-12-31T00:00:00\",\r\n    \"last_action\": null,\r\n    \"sol_instead_code\": \"1101\",\r\n    \"sol_instead_name\": \"กรุงเทพมหานคร\",\r\n    \"sol_hq\": \"1100\",\r\n    \"plans_000\": \"0\",\r\n    \"plansname_000\": \"\",\r\n    \"org_000\": \"\",\r\n    \"solid_000\": \"\",\r\n    \"orgname_000\": \"\",\r\n    \"org_748\": \"\",\r\n    \"solid_748\": \"\",\r\n    \"orgname_748\": \"\",\r\n    \"create_date\": \"2024-09-05T15:45:20.457\",\r\n    \"update_date\": \"2024-09-05T15:45:20.457\"\r\n  }\r\n  ]\r\n}";

            string result3 = "{\r\n  \"status\": true,\r\n  \"message\": \"พบข้อมูลจำนวน 1 record\",\r\n  \"data\": [\r\n   {\r\n    \"emp_code\": \"18499\",\r\n    \"fname\": \"ชื่อพนักงาน 18499\",\r\n    \"lname\": \"นามสกุลพนักงาน 18499\",\r\n    \"id_card\": \"\",\r\n    \"birth_date\": \"\",\r\n    \"grade\": \"10\",\r\n    \"emp_type\": \"2\",\r\n    \"position_id\": \"60039391\",\r\n    \"position_name\": \"หัวหน้าบริหารจัดการสาขา\",\r\n    \"email\": \"DOLAPORN.D@GHB.CO.TH\",\r\n    \"tel\": \"\",\r\n    \"org_level\": \"601\",\r\n    \"comp_objid\": \"50000001\",\r\n    \"comp_code\": \"D9100\",\r\n    \"comp_short_name\": \"ธอส.\",\r\n    \"comp_name\": \"ธนาคารอาคารสงเคราะห์\",\r\n    \"group_objid\": \"50004660\",\r\n    \"group_code\": \"D9100\",\r\n    \"group_short_name\": \"กง.นคร.\",\r\n    \"group_name\": \"กลุ่มงานสาขา\",\r\n    \"field_objid\": \"50004662\",\r\n    \"field_code\": \"D9100\",\r\n    \"field_short_name\": \"สง.นคร.\",\r\n    \"field_name\": \"สายงานสาขานครหลวง\",\r\n    \"dept_objid\": \"50004663\",\r\n    \"dept_code\": \"88802\",\r\n    \"dept_short_name\": \"สป.1\",\r\n    \"dept_name\": \"ฝ่ายสาขา กทม.และปริมณฑล 1\",\r\n    \"center_objid\": \"\",\r\n    \"center_code\": \"\",\r\n    \"center_short_name\": \"\",\r\n    \"center_name\": \"\",\r\n    \"zone_objid\": \"50005163\",\r\n    \"zone_code\": \"77022\",\r\n    \"zone_short_name\": \"สป.1(สนข.น.)\",\r\n    \"zone_name\": \"สำนักงานเขตกรุงเทพ-เหนือ\",\r\n    \"division_objid\": \"\",\r\n    \"division_code\": \"\",\r\n    \"division_short_name\": \"\",\r\n    \"division_name\": \"\",\r\n    \"sol_objid\": \"50004761\",\r\n    \"sol_code\": \"50000\",\r\n    \"sol_short_name\": \"สป.1(ดม.)\",\r\n    \"sol_name\": \"สาขาดอนเมือง\",\r\n    \"sub_sol_objid\": \"\",\r\n    \"sub_sol_code\": \"\",\r\n    \"sub_sol_short_name\": \"\",\r\n    \"sub_sol_name\": \"\",\r\n    \"start_date\": \"1996-06-03T00:00:00\",\r\n    \"stop_date\": \"9999-12-31T00:00:00\",\r\n    \"last_action\": null,\r\n    \"sol_instead_code\": \"1101\",\r\n    \"sol_instead_name\": \"กรุงเทพมหานคร\",\r\n    \"sol_hq\": \"1100\",\r\n    \"plans_000\": \"0\",\r\n    \"plansname_000\": \"\",\r\n    \"org_000\": \"\",\r\n    \"solid_000\": \"\",\r\n    \"orgname_000\": \"\",\r\n    \"org_748\": \"\",\r\n    \"solid_748\": \"\",\r\n    \"orgname_748\": \"\",\r\n    \"create_date\": \"2024-09-05T15:45:20.457\",\r\n    \"update_date\": \"2024-09-05T15:45:20.457\"\r\n  }\r\n  ]\r\n}";

            string result4 = "{\r\n  \"status\": true,\r\n  \"message\": \"พบข้อมูลจำนวน 1 record\",\r\n  \"data\": [\r\n   {\r\n\t\t\"emp_code\": \"53181\",\r\n\t\t\"fname\": \"ชื่อพนักงาน 53181\",\r\n\t\t\"lname\": \"นามสกุลพนักงาน 53181\",\r\n\t\t\"id_card\": \"\",\r\n\t\t\"birth_date\": \"\",\r\n\t\t\"grade\": \"7\",\r\n\t\t\"emp_type\": \"2\",\r\n\t\t\"position_id\": \"60003589\",\r\n\t\t\"position_name\": \"พนักงาน DEC อาวุโส\",\r\n\t\t\"email\": \"KAMOLCHANOK.A@GHB.CO.TH\",\r\n\t\t\"tel\": \"7069-70 ต่อ 203\",\r\n\t\t\"org_level\": \"600\",\r\n\t\t\"comp_objid\": \"50000001\",\r\n\t\t\"comp_code\": \"D9100\",\r\n\t\t\"comp_short_name\": \"ธอส.\",\r\n\t\t\"comp_name\": \"ธนาคารอาคารสงเคราะห์\",\r\n\t\t\"group_objid\": \"50004660\",\r\n\t\t\"group_code\": \"D9100\",\r\n\t\t\"group_short_name\": \"กง.นคร.\",\r\n\t\t\"group_name\": \"กลุ่มงานสาขา\",\r\n\t\t\"field_objid\": \"50004668\",\r\n\t\t\"field_code\": \"D9100\",\r\n\t\t\"field_short_name\": \"สง.ภภ.2\",\r\n\t\t\"field_name\": \"สายงานสาขาภูมิภาค 2\",\r\n\t\t\"dept_objid\": \"\",\r\n\t\t\"dept_code\": \"\",\r\n\t\t\"dept_short_name\": \"\",\r\n\t\t\"dept_name\": \"\",\r\n\t\t\"center_objid\": \"\",\r\n\t\t\"center_code\": \"\",\r\n\t\t\"center_short_name\": \"\",\r\n\t\t\"center_name\": \"\",\r\n\t\t\"zone_objid\": \"\",\r\n\t\t\"zone_code\": \"\",\r\n\t\t\"zone_short_name\": \"\",\r\n\t\t\"zone_name\": \"\",\r\n\t\t\"division_objid\": \"50000267\",\r\n\t\t\"division_code\": \"78483\",\r\n\t\t\"division_short_name\": \"ภ.อนล(DECอบ)\",\r\n\t\t\"division_name\": \"ศูนย์วิเคราะห์สินเชื่ออุบลราชธานี\",\r\n\t\t\"sol_objid\": \"\",\r\n\t\t\"sol_code\": \"\",\r\n\t\t\"sol_short_name\": \"\",\r\n\t\t\"sol_name\": \"\",\r\n\t\t\"sub_sol_objid\": \"\",\r\n\t\t\"sub_sol_code\": \"\",\r\n\t\t\"sub_sol_short_name\": \"\",\r\n\t\t\"sub_sol_name\": \"\",\r\n\t\t\"start_date\": \"2016-09-05T00:00:00\",\r\n\t\t\"stop_date\": \"9999-12-31T00:00:00\",\r\n\t\t\"last_action\": null,\r\n\t\t\"sol_instead_code\": \"1420\",\r\n\t\t\"sol_instead_name\": \"อุบลราชธานี\",\r\n\t\t\"sol_hq\": \"1400\",\r\n\t\t\"plans_000\": \"0\",\r\n\t\t\"plansname_000\": \"\",\r\n\t\t\"org_000\": \"\",\r\n\t\t\"solid_000\": \"\",\r\n\t\t\"orgname_000\": \"\",\r\n\t\t\"org_748\": \"\",\r\n\t\t\"solid_748\": \"\",\r\n\t\t\"orgname_748\": \"\",\r\n\t\t\"create_date\": \"2024-09-05T15:45:20.457\",\r\n\t\t\"update_date\": \"2024-09-05T15:45:20.457\"\r\n\t}\r\n  ]\r\n}";

            string result5 = "{\r\n  \"status\": true,\r\n  \"message\": \"พบข้อมูลจำนวน 1 record\",\r\n  \"data\": [\r\n   {\r\n\t\t\"emp_code\": \"12500\",\r\n\t\t\"fname\": \"ชื่อพนักงาน 12500\",\r\n\t\t\"lname\": \"นามสกุลพนักงาน 12500\",\r\n\t\t\"id_card\": \"\",\r\n\t\t\"birth_date\": \"\",\r\n\t\t\"grade\": \"8\",\r\n\t\t\"emp_type\": \"2\",\r\n\t\t\"position_id\": \"60038159\",\r\n\t\t\"position_name\": \"พนักงานบริหารหนี้อาวุโส\",\r\n\t\t\"email\": \"SUCHOKE.P@GHB.CO.TH\",\r\n\t\t\"tel\": \"\",\r\n\t\t\"org_level\": \"601\",\r\n\t\t\"comp_objid\": \"50000001\",\r\n\t\t\"comp_code\": \"D9100\",\r\n\t\t\"comp_short_name\": \"ธอส.\",\r\n\t\t\"comp_name\": \"ธนาคารอาคารสงเคราะห์\",\r\n\t\t\"group_objid\": \"50005237\",\r\n\t\t\"group_code\": \"D9100\",\r\n\t\t\"group_short_name\": \"กง.ปคส.\",\r\n\t\t\"group_name\": \"กลุ่มงานปรับโครงสร้างหนี้\",\r\n\t\t\"field_objid\": \"50004682\",\r\n\t\t\"field_code\": \"D9100\",\r\n\t\t\"field_short_name\": \"สง.บน.\",\r\n\t\t\"field_name\": \"สายงานบริหารหนี้\",\r\n\t\t\"dept_objid\": \"50004684\",\r\n\t\t\"dept_code\": \"88834\",\r\n\t\t\"dept_short_name\": \"บภ.\",\r\n\t\t\"dept_name\": \"ฝ่ายบริหารหนี้ภูมิภาค\",\r\n\t\t\"center_objid\": \"\",\r\n\t\t\"center_code\": \"\",\r\n\t\t\"center_short_name\": \"\",\r\n\t\t\"center_name\": \"\",\r\n\t\t\"zone_objid\": \"50000293\",\r\n\t\t\"zone_code\": \"88834\",\r\n\t\t\"zone_short_name\": \"บภ.(สบน.3-2)\",\r\n\t\t\"zone_name\": \"เขตสมุทรสาคร\",\r\n\t\t\"division_objid\": \"\",\r\n\t\t\"division_code\": \"\",\r\n\t\t\"division_short_name\": \"\",\r\n\t\t\"division_name\": \"\",\r\n\t\t\"sol_objid\": \"50000499\",\r\n\t\t\"sol_code\": \"88834\",\r\n\t\t\"sol_short_name\": \"สค.\",\r\n\t\t\"sol_name\": \"(ปฏิบัติหน้าที่สาขากาญจนบุรี)\",\r\n\t\t\"sub_sol_objid\": \"\",\r\n\t\t\"sub_sol_code\": \"\",\r\n\t\t\"sub_sol_short_name\": \"\",\r\n\t\t\"sub_sol_name\": \"\",\r\n\t\t\"start_date\": \"1993-06-01T00:00:00\",\r\n\t\t\"stop_date\": \"9999-12-31T00:00:00\",\r\n\t\t\"last_action\": null,\r\n\t\t\"sol_instead_code\": \"1106\",\r\n\t\t\"sol_instead_name\": \"สมุทรสาคร\",\r\n\t\t\"sol_hq\": \"1100\",\r\n\t\t\"plans_000\": \"0\",\r\n\t\t\"plansname_000\": \"\",\r\n\t\t\"org_000\": \"\",\r\n\t\t\"solid_000\": \"\",\r\n\t\t\"orgname_000\": \"\",\r\n\t\t\"org_748\": \"\",\r\n\t\t\"solid_748\": \"\",\r\n\t\t\"orgname_748\": \"\",\r\n\t\t\"create_date\": \"2024-09-05T15:45:20.457\",\r\n\t\t\"update_date\": \"2024-09-05T15:45:20.457\"\r\n\t}\r\n  ]\r\n}";

            string result6 = "{\r\n  \"status\": true,\r\n  \"message\": \"พบข้อมูลจำนวน 1 record\",\r\n  \"data\": [\r\n   {\r\n\t\t\"emp_code\": \"40022\",\r\n\t\t\"fname\": \"ชื่อพนักงาน 40022\",\r\n\t\t\"lname\": \"นามสกุลพนักงาน 40022\",\r\n\t\t\"id_card\": \"\",\r\n\t\t\"birth_date\": \"\",\r\n\t\t\"grade\": \"9\",\r\n\t\t\"emp_type\": \"2\",\r\n\t\t\"position_id\": \"60002721\",\r\n\t\t\"position_name\": \"พนักงานคอมพิวเตอร์อาวุโส\",\r\n\t\t\"email\": \"SUCHEEWA.K@GHB.CO.TH\",\r\n\t\t\"tel\": \"22022266\",\r\n\t\t\"org_level\": \"600\",\r\n\t\t\"comp_objid\": \"50000001\",\r\n\t\t\"comp_code\": \"D9100\",\r\n\t\t\"comp_short_name\": \"ธอส.\",\r\n\t\t\"comp_name\": \"ธนาคารอาคารสงเคราะห์\",\r\n\t\t\"group_objid\": \"50005238\",\r\n\t\t\"group_code\": \"D9100\",\r\n\t\t\"group_short_name\": \"กง.ทส.\",\r\n\t\t\"group_name\": \"กลุ่มงานเทคโนโลยีสารสนเทศ\",\r\n\t\t\"field_objid\": \"50004657\",\r\n\t\t\"field_code\": \"D9100\",\r\n\t\t\"field_short_name\": \"สง.พรท.\",\r\n\t\t\"field_name\": \"สายงานพัฒนาระบบดิจิทัล\",\r\n\t\t\"dept_objid\": \"50000261\",\r\n\t\t\"dept_code\": \"88855\",\r\n\t\t\"dept_short_name\": \"พด.\",\r\n\t\t\"dept_name\": \"ฝ่ายพัฒนาระบบดิจิทัลเซอร์วิส\",\r\n\t\t\"center_objid\": \"\",\r\n\t\t\"center_code\": \"\",\r\n\t\t\"center_short_name\": \"\",\r\n\t\t\"center_name\": \"\",\r\n\t\t\"zone_objid\": \"\",\r\n\t\t\"zone_code\": \"\",\r\n\t\t\"zone_short_name\": \"\",\r\n\t\t\"zone_name\": \"\",\r\n\t\t\"division_objid\": \"50000345\",\r\n\t\t\"division_code\": \"88855\",\r\n\t\t\"division_short_name\": \"พด.(สสท.)\",\r\n\t\t\"division_name\": \"ส่วนสนับสนุนและทดสอบระบบดิจิทัล\",\r\n\t\t\"sol_objid\": \"\",\r\n\t\t\"sol_code\": \"\",\r\n\t\t\"sol_short_name\": \"\",\r\n\t\t\"sol_name\": \"\",\r\n\t\t\"sub_sol_objid\": \"\",\r\n\t\t\"sub_sol_code\": \"\",\r\n\t\t\"sub_sol_short_name\": \"\",\r\n\t\t\"sub_sol_name\": \"\",\r\n\t\t\"start_date\": \"2012-06-05T00:00:00\",\r\n\t\t\"stop_date\": \"9999-12-31T00:00:00\",\r\n\t\t\"last_action\": null,\r\n\t\t\"sol_instead_code\": \"1001\",\r\n\t\t\"sol_instead_name\": \"กรุงเทพมหานคร\",\r\n\t\t\"sol_hq\": \"1000\",\r\n\t\t\"plans_000\": \"0\",\r\n\t\t\"plansname_000\": \"\",\r\n\t\t\"org_000\": \"\",\r\n\t\t\"solid_000\": \"\",\r\n\t\t\"orgname_000\": \"\",\r\n\t\t\"org_748\": \"\",\r\n\t\t\"solid_748\": \"\",\r\n\t\t\"orgname_748\": \"\",\r\n\t\t\"create_date\": \"2024-09-05T15:45:20.457\",\r\n\t\t\"update_date\": \"2024-09-05T15:45:20.457\"\r\n\t}\r\n  ]\r\n}";
            #endregion mock up

            #region token
            /*
            //************** oAuth/Token ***************
            using (var client = new HttpClient())
            {
                try
                {
                    var endPoint = new Uri(_iniCon.itUrl_v2);
                    //var result = client.GetAsync(endPoint).Result;
                    _logSys.WriteProcessLogFile(_strPathFile, "Uri(endPoint) : " + endPoint);
                    //var json = result.Content.ReadAsStringAsync();
                    //_logSys.WriteProcessLogFile(_strPathFile, "result.Content.ReadAsStringAsync() : " + json);

                    var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", _iniCon.itldap3_v2),
                        new KeyValuePair<string, string>("username", _iniCon.itldap1_v2),
                        new KeyValuePair<string, string>("password", _iniCon.itldap2_v2)
                    };

                    // สร้าง FormUrlEncodedContent จากพารามิเตอร์
                    HttpContent content = new FormUrlEncodedContent(parameters);
                    _logSys.WriteProcessLogFile(_strPathFile, "FormUrlEncodedContent : " + parameters);
                    // กำหนด Headers (ถ้ามี)
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                    // ส่ง POST Request ไปยัง API
                    //HttpResponseMessage response = await client.PostAsync(endPoint, content);
                    var result1 = client.PostAsync(endPoint, content).Result.Content.ReadAsStringAsync().Result;
                    _logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,content) : " + result1);
                    tokendata = JsonConvert.DeserializeObject<dynamic>(result1);//.Content.ReadAsStringAsync();
                    _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + tokendata);
                    //codeR.emp_code = "41830";
                }
                catch (Exception ex)
                {
                    _logSys.WriteProcessLogFile(_strPathFile, "Error HttpRequestV2 : " + ex.Message.ToString());
                }
            } */
            #endregion token

            #region Auth User
            /*
            //************** SignIn/Auth(User) ***************
            if (null != tokendata && userReq.username != "" && userReq.password != "")
            {
                _logSys.WriteProcessLogFile(_strPathFile, "*** Begin verified user *** ");
                var user_endPoint = new Uri(_iniCon.authenUrl_v2);
                _logSys.WriteProcessLogFile(_strPathFile, "user_endPoint : " + _iniCon.authenUrl);
                using (var client2 = new HttpClient())
                {
                    try
                    {
                        var userRequest = new UserPostRequest
                        {
                            username = userReq.username,
                            password = userReq.password
                        };
                        var user_newpostjson = JsonConvert.SerializeObject(userRequest);
                        //_logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + user_newpostjson + " / " + tokendata.access_token);
                        var user_playload = new StringContent(user_newpostjson, Encoding.UTF8, "application/json");
                        _logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + user_playload + " / " + user_playload.Headers);
                        string authorization = tokendata.access_token;
                        // Set the authorization header with a Bearer token  
                        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
                        _logSys.WriteProcessLogFile(_strPathFile, "authorization: " + tokendata.token_type + " " + tokendata.access_token);
                        //client2.DefaultRequestHeaders.Add("X-Auth-Token", authorization);
                        var result1 = client2.PostAsync(user_endPoint, user_playload).Result.Content.ReadAsStringAsync().Result;
                        _logSys.WriteProcessLogFile(_strPathFile, "client.PostAsync(endPoint,playload) : " + result1);
                        userdata = JsonConvert.DeserializeObject<dynamic>(result1);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + userdata);
                        var chkStu = userdata.status.ToString();
                        if (chkStu.Trim().ToLower() != "false")
                        {
                            var dt = userdata.data;

                            ldapUser.emp_code = dt[0].emp_code;
                            ldapUser.fname = dt[0].fname;
                            ldapUser.lname = dt[0].lname;
                            ldapUser.id_card = dt[0].id_card;
                            ldapUser.birth_date = dt[0].birth_date;
                            ldapUser.grade = dt[0].grade;
                            ldapUser.emp_type = dt[0].emp_type;
                            ldapUser.position_id = dt[0].position_id;
                            ldapUser.position_name = dt[0].position_name;
                            ldapUser.email = dt[0].email;
                            ldapUser.tel = dt[0].tel;
                            ldapUser.org_level = dt[0].org_level;
                            ldapUser.comp_objid = dt[0].comp_objid;
                            ldapUser.comp_code = dt[0].comp_code;
                            ldapUser.comp_short_name = dt[0].comp_short_name;
                            ldapUser.comp_name = dt[0].comp_name;
                            ldapUser.group_objid = dt[0].group_objid;
                            ldapUser.group_code = dt[0].group_code;
                            ldapUser.group_short_name = dt[0].group_short_name;
                            ldapUser.group_name = dt[0].group_name;
                            ldapUser.field_objid = dt[0].field_objid;
                            ldapUser.field_code = dt[0].field_code;
                            ldapUser.field_short_name = dt[0].field_short_name;
                            ldapUser.field_name = dt[0].field_name;
                            ldapUser.dept_objid = dt[0].dept_objid;
                            ldapUser.dept_code = dt[0].dept_code;
                            ldapUser.dept_short_name = dt[0].dept_short_name;
                            ldapUser.dept_name = dt[0].dept_name;
                            ldapUser.center_objid = dt[0].center_objid;
                            ldapUser.center_code = dt[0].center_code;
                            ldapUser.center_short_name = dt[0].center_short_name;
                            ldapUser.center_name = dt[0].center_name;
                            ldapUser.zone_objid = dt[0].zone_objid;
                            ldapUser.zone_code = dt[0].zone_code;
                            ldapUser.zone_short_name = dt[0].zone_short_names;
                            ldapUser.zone_name = dt[0].zone_name;
                            ldapUser.division_objid = dt[0].division_objid;
                            ldapUser.division_code = dt[0].division_code;
                            ldapUser.division_short_name = dt[0].division_short_name;
                            ldapUser.division_name = dt[0].division_name;
                            ldapUser.sol_objid = dt[0].sol_objid;
                            ldapUser.sol_code = dt[0].sol_code;
                            ldapUser.sol_short_name = dt[0].sol_short_name;
                            ldapUser.sol_name = dt[0].sol_name;
                            ldapUser.sub_sol_objid = dt[0].sub_sol_objid;
                            ldapUser.sub_sol_code = dt[0].sub_sol_code;
                            ldapUser.sub_sol_short_name = dt[0].sub_sol_short_name;
                            ldapUser.sub_sol_name = dt[0].sub_sol_name;
                            ldapUser.start_date = dt[0].start_date;
                            ldapUser.stop_date = dt[0].stop_date;
                            ldapUser.last_action = dt[0].last_action;
                            ldapUser.sol_instead_code = dt[0].sol_instead_code;
                            ldapUser.sol_instead_name = dt[0].sol_instead_name;
                            ldapUser.sol_hq = dt[0].sol_hq;
                            ldapUser.plans_000 = dt[0].plans_000;
                            ldapUser.plansname_000 = dt[0].plansname_000;
                            ldapUser.org_000 = dt[0].org_000;
                            ldapUser.solid_000 = dt[0].solid_000;
                            ldapUser.orgname_000 = dt[0].orgname_000;
                            ldapUser.org_748 = dt[0].org_748;
                            ldapUser.solid_748 = dt[0].solid_748;
                            ldapUser.orgname_748 = dt[0].orgname_748;
                            ldapUser.create_date = dt[0].create_date;
                            ldapUser.update_date = dt[0].update_date;
                            _logSys.WriteProcessLogFile(_strPathFile, "add User Details  " + ldapUser.fname);
                            _logSys.WriteProcessLogFile(_strPathFile, "Show Details userdata.empCode : " + userdata.emp_Code + " / userdata.lName : " + userdata.lName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "This is not User ldap.  ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
                    }
                }
            }
            */
            #endregion Auth User

            #region Auth Code
            //************** Emp/Code ***************

            if (codeR.emp_code != "")
            {
                _logSys.WriteProcessLogFile(_strPathFile, "*** Begin verified with code *** ");
                var user_endPoint = new Uri(_iniCon.codeUrl_v2);
                _logSys.WriteProcessLogFile(_strPathFile, "user_endPoint : " + _iniCon.codeUrl_v2);
                using (var client2 = new HttpClient())
                {
                    try
                    {
                        //var userCodeRequest = new UserPostCodeRequest
                        //{
                        //    emp_code = codeR.emp_code
                        //};
                        //var user_newpostjson = JsonConvert.SerializeObject(userCodeRequest);
                        //_logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.SerializeObject(postRequest) : " + user_newpostjson + " / " + tokendata.access_token);
                        //var user_playload = new StringContent(user_newpostjson, Encoding.UTF8, "application/json");
                        //_logSys.WriteProcessLogFile(_strPathFile, "StringContent(newpostjson, Encoding.UTF8, 'application / json') : " + user_playload + " / " + user_playload.Headers);
                        //string authorization = tokendata.access_token;
                        //// Set the authorization header with a Bearer token  
                        //client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
                        //_logSys.WriteProcessLogFile(_strPathFile, "authorization: " + tokendata.token_type + " " + tokendata.access_token);
                        ////client2.DefaultRequestHeaders.Add("X-Auth-Token", authorization);
                        //var result1 = client2.PostAsync(user_endPoint, user_playload).Result.Content.ReadAsStringAsync().Result;
                        //_logSys.WriteProcessLogFile(_strPathFile, "client2.PostAsync(user_endPoint,user_playload) : " + result1);
                        userdata = JsonConvert.DeserializeObject<dynamic>(result6);
                        _logSys.WriteProcessLogFile(_strPathFile, "JsonConvert.DeserializeObject<dynamic>(result1) : " + userdata);
                        var chkStu = userdata.status.ToString();
                        //if (null != userdata)
                        if (chkStu.Trim().ToLower() != "false")
                        {
                            var dt = userdata.data;

                            ldapUser.emp_code = dt[0].emp_code;
                            ldapUser.fname = dt[0].fname;
                            ldapUser.lname = dt[0].lname;
                            ldapUser.id_card = dt[0].id_card;
                            ldapUser.birth_date = dt[0].birth_date;
                            ldapUser.grade = dt[0].grade;
                            ldapUser.emp_type = dt[0].emp_type;
                            ldapUser.position_id = dt[0].position_id;
                            ldapUser.position_name = dt[0].position_name;
                            ldapUser.email = dt[0].email;
                            ldapUser.tel = dt[0].tel;
                            ldapUser.org_level = dt[0].org_level;
                            ldapUser.comp_objid = dt[0].comp_objid;
                            ldapUser.comp_code = dt[0].comp_code;
                            ldapUser.comp_short_name = dt[0].comp_short_name;
                            ldapUser.comp_name = dt[0].comp_name;
                            ldapUser.group_objid = dt[0].group_objid;
                            ldapUser.group_code = dt[0].group_code;
                            ldapUser.group_short_name = dt[0].group_short_name;
                            ldapUser.group_name = dt[0].group_name;
                            ldapUser.field_objid = dt[0].field_objid;
                            ldapUser.field_code = dt[0].field_code;
                            ldapUser.field_short_name = dt[0].field_short_name;
                            ldapUser.field_name = dt[0].field_name;
                            ldapUser.dept_objid = dt[0].dept_objid;
                            ldapUser.dept_code = dt[0].dept_code;
                            ldapUser.dept_short_name = dt[0].dept_short_name;
                            ldapUser.dept_name = dt[0].dept_name;
                            ldapUser.center_objid = dt[0].center_objid;
                            ldapUser.center_code = dt[0].center_code;
                            ldapUser.center_short_name = dt[0].center_short_name;
                            ldapUser.center_name = dt[0].center_name;
                            ldapUser.zone_objid = dt[0].zone_objid;
                            ldapUser.zone_code = dt[0].zone_code;
                            ldapUser.zone_short_name = dt[0].zone_short_names;
                            ldapUser.zone_name = dt[0].zone_name;
                            ldapUser.division_objid = dt[0].division_objid;
                            ldapUser.division_code = dt[0].division_code;
                            ldapUser.division_short_name = dt[0].division_short_name;
                            ldapUser.division_name = dt[0].division_name;
                            ldapUser.sol_objid = dt[0].sol_objid;
                            ldapUser.sol_code = dt[0].sol_code;
                            ldapUser.sol_short_name = dt[0].sol_short_name;
                            ldapUser.sol_name = dt[0].sol_name;
                            ldapUser.sub_sol_objid = dt[0].sub_sol_objid;
                            ldapUser.sub_sol_code = dt[0].sub_sol_code;
                            ldapUser.sub_sol_short_name = dt[0].sub_sol_short_name;
                            ldapUser.sub_sol_name = dt[0].sub_sol_name;
                            ldapUser.start_date = dt[0].start_date;
                            ldapUser.stop_date = dt[0].stop_date;
                            ldapUser.last_action = dt[0].last_action;
                            ldapUser.sol_instead_code = dt[0].sol_instead_code;
                            ldapUser.sol_instead_name = dt[0].sol_instead_name;
                            ldapUser.sol_hq = dt[0].sol_hq;
                            ldapUser.plans_000 = dt[0].plans_000;
                            ldapUser.plansname_000 = dt[0].plansname_000;
                            ldapUser.org_000 = dt[0].org_000;
                            ldapUser.solid_000 = dt[0].solid_000;
                            ldapUser.orgname_000 = dt[0].orgname_000;
                            ldapUser.org_748 = dt[0].org_748;
                            ldapUser.solid_748 = dt[0].solid_748;
                            ldapUser.orgname_748 = dt[0].orgname_748;
                            ldapUser.create_date = dt[0].create_date;
                            ldapUser.update_date = dt[0].update_date;
                            _logSys.WriteProcessLogFile(_strPathFile, "add User Details  " + ldapUser.fname);
                            _logSys.WriteProcessLogFile(_strPathFile, "Show Details userdata.empCode : " + userdata.emp_Code + " / userdata.lName : " + userdata.lName);
                        }
                        else
                        {
                            _logSys.WriteProcessLogFile(_strPathFile, "This is not User ldap.  ");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteProcessLogFile(_strPathFile, "Error GetUserByUserName2 : " + ex.Message.ToString());
                    }
                }
            }
            #endregion Auth Code

            return ldapUser;
        }
    }
}
