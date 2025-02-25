﻿using BoincRpc;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PSBoinc
{
    [Cmdlet(VerbsCommon.Get, "BoincTask")]
    [OutputType(typeof(Result))]
    public class GetBoincTaskCommand : RpcCmdlet
    {
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string[] WorkunitName { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string[] Project { get; set; }

        protected override void RpcProcessRecord()
        {
            Result[] results = RpcClient.GetResults();
            Project[] projects = RpcClient.GetProjectStatus();

            if (WorkunitName != null)
                results = Utils.FilterByName(results, r => r.WorkunitName, WorkunitName, "Could not find a task with name \"{0}\".", "NoTaskFoundForGivenName", this);

            if (Project != null && results.Length != 0)
            {
                HashSet<string> masterUrls = new HashSet<string>(
                    Utils.FilterByName(projects, p => p.ProjectName, Project, "Could not find a project with name \"{0}\".", "NoProjectFoundForGivenName", this)
                    .Select(p => p.MasterUrl));

                results = results.Where(r => masterUrls.Contains(r.ProjectUrl)).ToArray();
            }

            foreach (Result result in results){
                result.ProjectName = projects.Where(p => p.MasterUrl == result.ProjectUrl).Select(p => p.ProjectName).First();
            }

            WriteObject(results, true);
        }
    }
}
