using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHB_D1.Models
{

    public class ApiPostRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string granttype { get; set; }
    }

    public class UserPostRequest
    {
        public string username { get; set; }

        public string password { get; set; }
    }

    public class UserPostCodeRequest
    {
        public string emp_code { get; set; }
    }
    public class ApiPostResponse
    {
        public string token { get; set; }
        public string empCode { get; set; }
        public string fullName { get; set; }
        public string grade { get; set; }
        public string email { get; set; }
        public string departmentCode { get; set; }
        public string department { get; set; }
        public string employeeTypeCode { get; set; }
        public string employeeType { get; set; }
        public string division { get; set; }
        public string position { get; set; }
        public string hubCode { get; set; }
        public string hubName { get; set; }
        public string deCode { get; set; }
        public string deName { get; set; }
        public string solCode { get; set; }
        public string solName { get; set; }
    }

    public class ApiPostResponse2
    {
        public string emp_code { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string id_card { get; set; }
        public string birth_date { get; set; }
        public string grade { get; set; }
        public string emp_type { get; set; }
        public string position_id { get; set; }
        public string position_name { get; set; }
        public string email { get; set; }
        public string tel { get; set; }
        public string org_level { get; set; }
        public string comp_objid { get; set; }
        public string comp_code { get; set; }
        public string comp_short_name { get; set; }
        public string comp_name { get; set; }
        public string group_objid { get; set; }
        public string group_code { get; set; }
        public string group_short_name { get; set; }
        public string group_name { get; set; }
        public string field_objid { get; set; }
        public string field_code { get; set; }
        public string field_short_name { get; set; }
        public string field_name { get; set; }
        public string dept_objid { get; set; }
        public string dept_code { get; set; }
        public string dept_short_name { get; set; }
        public string dept_name { get; set; }
        public string center_objid { get; set; }
        public string center_code { get; set; }
        public string center_short_name { get; set; }
        public string center_name { get; set; }
        public string zone_objid { get; set; }
        public string zone_code { get; set; }
        public string zone_short_name { get; set; }
        public string zone_name { get; set; }
        public string division_objid { get; set; }
        public string division_code { get; set; }
        public string division_short_name { get; set; }
        public string division_name { get; set; }
        public string sol_objid { get; set; }
        public string sol_code { get; set; }
        public string sol_short_name { get; set; }
        public string sol_name { get; set; }
        public string sub_sol_objid { get; set; }
        public string sub_sol_code { get; set; }
        public string sub_sol_short_name { get; set; }
        public string sub_sol_name { get; set; }
        public string start_date { get; set; }
        public string stop_date { get; set; }
        public string last_action { get; set; }
        public string sol_instead_code { get; set; }
        public string sol_instead_name { get; set; }
        public string sol_hq { get; set; }
        public string plans_000 { get; set; }
        public string plansname_000 { get; set; }
        public string org_000 { get; set; }
        public string solid_000 { get; set; }
        public string orgname_000 { get; set; }
        public string org_748 { get; set; }
        public string solid_748 { get; set; }
        public string orgname_748 { get; set; }
        public string create_date { get; set; }
        public string update_date { get; set; }
    }
    public class GHBCoreAPIModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string granttype { get; set; }
        public string code { get; set; }
        public string postUrl { get; set; }
        public string empCode { get; set; }
        public string fullName { get; set; }
        public string grade { get; set; }
        public string email { get; set; }
        public string departmentCode { get; set; }
        public string department { get; set; }
        public string employeeTypeCode { get; set; }
        public string employeeType { get; set; }
        public string division { get; set; }
        public string position { get; set; }
        public string hubCode { get; set; }
        public string hubName { get; set; }
        public string deCode { get; set; }
        public string deName { get; set; }
        public string solCode { get; set; }
        public string solName { get; set; }

    }

    public class RequestBody
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        [JsonProperty("audience")]
        public string Audience { get; set; }
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("realm")]
        public string Realm { get; set; }
    }

    public class ResponseBody
    {
        [JsonProperty("access_token")]
        public string AccessToken;
        [JsonProperty("id_token")]
        public string IdToken;
        [JsonProperty("expires_in")]
        public string ExpiresIn;
        [JsonProperty("scope")]
        public string Scope;
        [JsonProperty("token_type")]
        public string TokenType;
    }

}