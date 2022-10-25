﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using TightWiki.Shared.Library;
using TightWiki.Shared.Models;
using TightWiki.Shared.Models.Data;
using TightWiki.Shared.Repository;
using TightWiki.Shared.Wiki;
using static TightWiki.Shared.Wiki.MethodCall.Singletons;

namespace TightWiki.Controllers
{
    public class ControllerHelperBase : Controller
    {
        public StateContext context = new StateContext();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Configure();
        }

        public void Configure()
        {
            HydrateSecurityContext();

            var basicConfig = ConfigurationRepository.GetConfigurationEntryValuesByGroupName("Basic");
            var htmlConfig = ConfigurationRepository.GetConfigurationEntryValuesByGroupName("HTML Layout");
            var functConfig = ConfigurationRepository.GetConfigurationEntryValuesByGroupName("Functionality");

            ViewBag.Config = new ViewBagConfig
            {
                DefaultTimeZone = basicConfig.As<string>("Default TimeZone"),
                Context = context,
                PathAndQuery = Request.GetEncodedPathAndQuery(),
                IncludeWikiDescriptionInMeta = functConfig.As<bool>("Include wiki Description in Meta"),
                IncludeWikiTagsInMeta = functConfig.As<bool>("Include wiki Tags in Meta"),
                HTMLHeader = htmlConfig.As<string>("Header"),
                HTMLFooter = htmlConfig.As<string>("Footer"),
                HTMLPreBody = htmlConfig.As<string>("Pre-Body"),
                HTMLPostBody = htmlConfig.As<string>("Post-Body"),
                BrandImageSmall = basicConfig.As<string>("Brand Image (Small)"),
                Name = basicConfig.As<string>("Name"),
                Title = basicConfig.As<string>("Name"), //Default the title to the name. This will be replaced when the page is found and loaded.
                FooterBlurb = basicConfig.As<string>("FooterBlurb"),
                Copyright = basicConfig.As<string>("Copyright"),
                PageNavigation = RouteValue("pageNavigation"),
                PageRevision = RouteValue("pageRevision"),
                MenuItems = MenuItemRepository.GetAllMenuItems()
            };

            context.Config = ViewBag.Config;
            ViewBag.Context = context;
        }

        public void HydrateSecurityContext()
        {
            context.IsAuthenticated = false;

            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
                var roles = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                context.IsAuthenticated = User.Identity.IsAuthenticated;
                if (context.IsAuthenticated)
                {
                    context.Roles = roles.ToList();
                    context.User = UserRepository.GetUserById(userId);
                }
            }
        }

        public int SavePage(Page page)
        {
            page.Id = PageRepository.SavePage(page);

            var wikifier = new Wikifier(context, page, null, Request.Query, new WikiMatchType[] { WikiMatchType.Function });
            PageTagRepository.UpdatePageTags(page.Id, wikifier.Tags);
            PageRepository.UpdatePageProcessingInstructions(page.Id, wikifier.ProcessingInstructions);
            var pageTokens = wikifier.ParsePageTokens().Select(o => o.ToPageToken(page.Id)).ToList();
            PageRepository.SavePageTokens(pageTokens);

            Cache.ClearClass($"Page:{page.Navigation}");

            return page.Id;
        }

        public void PerformLogin(string emailAddress, string password, bool isPasswordHash)
        {
            var requireEmailVerification = ConfigurationRepository.Get<bool>("Membership", "Require Email Verification");

            User user;

            if (isPasswordHash)
            {
                user = UserRepository.GetUserByEmailAndPasswordHash(emailAddress, password);
            }
            else
            {
                user = UserRepository.GetUserByEmailAndPassword(emailAddress, password);
            }

            if (user != null)
            {
                if (requireEmailVerification == true && user.EmailVerified == false)
                {
                    throw new Exception("Email address has not been verified. Check your email or use the password reset link to confirm your email address.");
                }

                var roles = UserRepository.GetUserRolesByUserId(user.Id);
                string arrayOfRoles = string.Join("|", roles.Select(o => o.Name));

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.EmailAddress),
                    new Claim(ClaimTypes.Name, user.AccountName),
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, arrayOfRoles)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Login");
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                UserRepository.UpdateUserLastLoginDateByUserId(user.Id);
            }
            else
            {
                throw new Exception("Invalid Username or Password");
            }
        }

        public string RouteValue(string key, string defaultValue = "")
        {
            if (RouteData.Values.ContainsKey(key))
            {
                return RouteData.Values[key]?.ToString();
            }
            return defaultValue;
        }
    }
}
