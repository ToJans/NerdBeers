using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Nancy;
using System.IO;

namespace Org.NerdBeers.Specs.Modules
{
    public class Login_with_a_cookie : with_NerdBeersContext
    {
        Establish context = () => 
        {
            Req = new Nancy.Request("POST", "/", "text/html");
        };

        Because of = () => ProcessRequest();

        It should_have_a_current_nerd_which_matches_the_cookie_id;
    }

    public class Login_with_an_existing_email_and_matching_password : with_NerdBeersContext
    {
        Establish context = () =>
        {
            Req = new Nancy.Request("POST", "/login", "text/html");
            Req.Form.Email = "a@b.com";
            Req.Form.Password = "secret";
        };
        Because of = () => ProcessRequest();

        It should_have_a_matching_current_nerd;
        It should_redirect_to_the_original_url;
        It should_show_a_welcome_message;
        It should_allow_the_nerd_to_modify_its_data;
    }

    public class Login_with_an_existing_email_and_mismatching_password : with_NerdBeersContext
    {
        Establish context = () =>
        {
            Req = new Nancy.Request("POST", "/login", "text/html");
            Req.Form.Email = "a@b.com";
            Req.Form.Password = "wrongsecret";
        };

        Because of = () => ProcessRequest();

        It should_not_have_a_matching_current_nerd;
        It should_show_an_error_message;
        It should_allow_the_nerd_to_request_a_new_password;
    }

    public class Login_with_a_new_email : with_NerdBeersContext
    {
        Establish context = () =>
        {
            Req = new Nancy.Request("POST", "/login", "text/html");
            Req.Form.Email = "c@d.com";
            Req.Form.Password = "wrongsecret";
        };

        Because of = () => ProcessRequest();

        It should_not_have_a_matching_current_nerd;
        It should_show_an_error_message;
        It should_allow_the_nerd_to_register_himself;
    }

    public class Logout : with_NerdBeersContext
    {
        Establish context = () =>
        {
            Req.Cookies["NerdGuid"] = DB.Nerds.FindById(1).Guid;
            Req = new Nancy.Request("POST", "/logout", "text/html");
        };
        Because of = () => ProcessRequest();

        It should_not_have_a_matching_current_nerd;
        It should_show_a_logout_message;
        It should_allow_the_nerd_to_login;
    }
}
