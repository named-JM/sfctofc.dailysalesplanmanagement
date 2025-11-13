// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using System.Security.Claims;
global using AutoMapper;
global using AutoMapper.QueryableExtensions;
global using SFCTOFC.DailySalesPlanManagement.Application.Common.Interfaces;
global using SFCTOFC.DailySalesPlanManagement.Application.Common.Interfaces.Identity;
global using SFCTOFC.DailySalesPlanManagement.Application.Common.Models;
global using SFCTOFC.DailySalesPlanManagement.Infrastructure.Persistence;
global using SFCTOFC.DailySalesPlanManagement.Infrastructure.Persistence.Extensions;
global using SFCTOFC.DailySalesPlanManagement.Infrastructure.Services;
global using SFCTOFC.DailySalesPlanManagement.Infrastructure.Services.Identity;
global using SFCTOFC.DailySalesPlanManagement.Domain.Entities;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;