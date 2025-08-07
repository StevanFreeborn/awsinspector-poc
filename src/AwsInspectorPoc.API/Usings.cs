global using System.Security.Claims;
global using System.Text;
global using System.Text.Encodings.Web;
global using System.Threading.Channels;

global using Amazon;
global using Amazon.ResourceExplorer2;
global using Amazon.ResourceExplorer2.Model;
global using Amazon.Runtime;

global using AwsInspectorPoc.API.Authentication;
global using AwsInspectorPoc.API.Models;
global using AwsInspectorPoc.API.Monitors;
global using AwsInspectorPoc.API.Options;
global using AwsInspectorPoc.API.Queues;
global using AwsInspectorPoc.API.Services;
global using AwsInspectorPoc.API.Telemetry;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Options;

global using Onspring.API.SDK;
global using Onspring.API.SDK.Models;

global using OpenTelemetry.Exporter;
global using OpenTelemetry.Logs;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
