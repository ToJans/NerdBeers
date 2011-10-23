using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Nancy;
using Nancy.Testing;
using System.IO;

namespace Org.NerdBeers.Specs.Modules
{
    public class Login_with_a_cookie : with_NerdBeersContext
    {
        Because of = () => result = browser.Get("/", with => with.HttpRequest());

        It should_have_a_current_nerd_which_matches_the_cookie_id;
    }

    public class Login_with_an_existing_email_and_matching_password : with_NerdBeersContext
    {
        Because of = () =>
            {
                result = browser.Post("/authentication/login",with=> {
                    with.HttpRequest();
                    with.FormValue("Email","a@b.com");
                    with.FormValue("Password","secret");
                });
            };

        It should_have_a_matching_current_nerd;
        It should_redirect_to_the_original_url;
        It should_show_a_welcome_message;
        It should_allow_the_nerd_to_modify_its_data;
    }

    public class Login_with_an_existing_email_and_mismatching_password : with_NerdBeersContext
    {
        Because of = () =>
        {
            result = browser.Post("/authentication/login", with =>
            {
                with.HttpRequest();
                with.FormValue("Email", "a@b.com");
                with.FormValue("Password", "wrongsecret");
            });
        };

        It should_not_have_a_matching_current_nerd;
        It should_show_an_error_message;
        It should_allow_the_nerd_to_request_a_new_password;
    }

    public class Login_with_a_new_email : with_NerdBeersContext
    {
        Because of = () =>
        {
            result = browser.Post("/authentication/login", with =>
            {
                with.HttpRequest();
                with.FormValue("Email", "c@d.com");
                with.FormValue("Password", "secret");
            });
        };

        It should_not_have_a_matching_current_nerd;
        It should_show_an_error_message;
        It should_allow_the_nerd_to_register_himself;
    }

    public class Logout : with_NerdBeersContext
    {
        Because of = () => {
            result = browser.Post("/authentication/logout", with => {
                with.HttpRequest();
                with.Header("Cookie", "NerdGuid=" + DB.Nerds.FindById(1).Guid.ToString());
            });
        };

        It should_not_have_a_matching_current_nerd;
        It should_show_a_logout_message;
        It should_allow_the_nerd_to_login;
    }
}
