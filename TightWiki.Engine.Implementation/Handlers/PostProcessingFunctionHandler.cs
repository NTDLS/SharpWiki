﻿using System.Text;
using TightWiki.Engine.Function;
using TightWiki.Engine.Implementation.Utility;
using TightWiki.Engine.Library;
using TightWiki.Engine.Library.Interfaces;
using TightWiki.Models;
using static TightWiki.Engine.Function.FunctionPrototypeCollection;
using static TightWiki.Engine.Library.Constants;

namespace TightWiki.Engine.Implementation.Handlers
{
    /// <summary>
    /// Handles post-processing function calls.
    /// </summary>
    public class PostProcessingFunctionHandler : IPostProcessingFunctionHandler
    {
        private static FunctionPrototypeCollection? _collection;

        public FunctionPrototypeCollection Prototypes
        {
            get
            {
                if (_collection == null)
                {
                    _collection = new FunctionPrototypeCollection(WikiFunctionType.Standard);

                    // Pattern:
                    //  ##FunctionName: <parameterType>{parameterName(valid,parameter,values)}='defaultValue' | <string>{titleText}=''
                    // Notes:
                    //  Parameters should be separated by a pipe character: |
                    //  Parameters that are required are surrounded with [].
                    //  Parameters that are optional are surrounded with {}.
                    //  Parameters that only allow specific values should list the values after the parameter name in parentheses. e.g. paramName(valid,parameter,values)
                    //  All optional parameters should have a default value, even if its null.
                    //  All default values should be enclosed in single quotes (e.g. 'value') with the exception of NULL.


                    _collection.Add("##Tags: <string>{styleName(Flat,List)}='List'");
                    _collection.Add("##TagCloud: <string>[pageTag] | <integer>{Top}='1000'");
                    _collection.Add("##SearchCloud: <string>[searchPhrase] | <integer>{Top}='1000'");
                    _collection.Add("##TOC:<bool>{alphabetized}='false'");
                }

                return _collection;
            }
        }

        /// <summary>
        /// Called to handle function calls when proper prototypes are matched.
        /// </summary>
        /// <param name="state">Reference to the wiki state object</param>
        /// <param name="function">The parsed function call and all its parameters and their values.</param>
        /// <param name="scopeBody">This is not a scope function, this should always be null</param>
        public HandlerResult Handle(ITightEngineState state, FunctionCall function, string? scopeBody = null)
        {
            switch (function.Name.ToLower())
            {
                //------------------------------------------------------------------------------------------------------------------------------
                //Displays a tag link list.
                case "tags": //##tags
                    {
                        string styleName = function.Parameters.Get<string>("styleName").ToLower();
                        var html = new StringBuilder();

                        if (styleName == "list")
                        {
                            html.Append("<ul>");
                            foreach (var tag in state.Tags)
                            {
                                html.Append($"<li><a href=\"{GlobalConfiguration.BasePath}/Tag/Browse/{tag}\">{tag}</a>");
                            }
                            html.Append("</ul>");
                        }
                        else if (styleName == "flat")
                        {
                            foreach (var tag in state.Tags)
                            {
                                if (html.Length > 0) html.Append(" | ");
                                html.Append($"<a href=\"{GlobalConfiguration.BasePath}/Tag/Browse/{tag}\">{tag}</a>");
                            }
                        }

                        return new HandlerResult(html.ToString());
                    }

                //------------------------------------------------------------------------------------------------------------------------------
                case "tagcloud":
                    {
                        var top = function.Parameters.Get<int>("Top");
                        string seedTag = function.Parameters.Get<string>("pageTag");

                        string html = TagCloud.Build(seedTag, top);
                        return new HandlerResult(html);
                    }

                //------------------------------------------------------------------------------------------------------------------------------
                case "searchcloud":
                    {
                        var top = function.Parameters.Get<int>("Top");
                        var tokens = function.Parameters.Get<string>("searchPhrase").Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

                        string html = SearchCloud.Build(tokens, top);
                        return new HandlerResult(html);
                    }

                //------------------------------------------------------------------------------------------------------------------------------
                //Displays a table of contents for the page based on the header tags.
                case "toc":
                    {
                        bool alphabetized = function.Parameters.Get<bool>("alphabetized");

                        var html = new StringBuilder();

                        var tags = (from t in state.TableOfContents
                                    orderby t.StartingPosition
                                    select t).ToList();

                        var unordered = new List<TableOfContentsTag>();
                        var ordered = new List<TableOfContentsTag>();

                        if (alphabetized)
                        {
                            int level = tags.FirstOrDefault()?.Level ?? 0;

                            foreach (var tag in tags)
                            {
                                if (level != tag.Level)
                                {
                                    ordered.AddRange(unordered.OrderBy(o => o.Text));
                                    unordered.Clear();
                                    level = tag.Level;
                                }

                                unordered.Add(tag);
                            }

                            ordered.AddRange(unordered.OrderBy(o => o.Text));
                            unordered.Clear();

                            tags = ordered.ToList();
                        }

                        int currentLevel = 0;

                        foreach (var tag in tags)
                        {
                            if (tag.Level > currentLevel)
                            {
                                while (currentLevel < tag.Level)
                                {
                                    html.Append("<ul>");
                                    currentLevel++;
                                }
                            }
                            else if (tag.Level < currentLevel)
                            {
                                while (currentLevel > tag.Level)
                                {

                                    html.Append("</ul>");
                                    currentLevel--;
                                }
                            }

                            html.Append("<li><a href=\"#" + tag.HrefTag + "\">" + tag.Text + "</a></li>");
                        }

                        while (currentLevel > 0)
                        {
                            html.Append("</ul>");
                            currentLevel--;
                        }

                        return new HandlerResult(html.ToString());
                    }
            }

            return new HandlerResult() { Instructions = [HandlerResultInstruction.Skip] };
        }
    }
}