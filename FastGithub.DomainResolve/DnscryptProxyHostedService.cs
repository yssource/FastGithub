﻿using FastGithub.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FastGithub.DomainResolve
{
    /// <summary>
    /// DnscryptProxy后台服务
    /// </summary>
    sealed class DnscryptProxyHostedService : IHostedService
    {
        private readonly FastGithubConfig fastGithubConfig;
        private readonly ILogger<DnscryptProxyHostedService> logger;
        private DnscryptProxy? dnscryptProxy;

        /// <summary>
        /// DnscryptProxy后台服务
        /// </summary>
        /// <param name="fastGithubConfig"></param>
        /// <param name="logger"></param>
        public DnscryptProxyHostedService(
            FastGithubConfig fastGithubConfig,
            ILogger<DnscryptProxyHostedService> logger)
        {
            this.fastGithubConfig = fastGithubConfig;
            this.logger = logger;
        }

        /// <summary>
        /// 启动dnscrypt-proxy
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var pureDns = this.fastGithubConfig.PureDns;
            if (LocalMachine.ContainsIPAddress(pureDns.Address) == true)
            {
                this.dnscryptProxy = new DnscryptProxy(pureDns);
                try
                {
                    await this.dnscryptProxy.StartAsync(cancellationToken);
                    this.logger.LogInformation($"{this.dnscryptProxy}启动成功");
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning($"{this.dnscryptProxy}启动失败：{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 停止dnscrypt-proxy
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.dnscryptProxy != null)
            {
                try
                {
                    this.dnscryptProxy.Stop();
                    this.logger.LogInformation($"{this.dnscryptProxy}已停止");
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning($"{this.dnscryptProxy}停止失败：{ex.Message}");
                }
            }

            return Task.CompletedTask;
        }
    }
}