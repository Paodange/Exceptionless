﻿using System;
using System.Collections.Generic;
using Exceptionless.Core.Pipeline;
using Exceptionless.Core.Extensions;
using Exceptionless.Core.Models;
using Exceptionless.Core.Queues.Models;

namespace Exceptionless.Core.Plugins.Formatting {
    [Priority(40)]
    public sealed class UsageFormattingPlugin : FormattingPluginBase {
        private bool ShouldHandle(PersistentEvent ev) {
            return ev.IsFeatureUsage();
        }

        public override SummaryData GetStackSummaryData(Stack stack) {
            if (!stack.SignatureInfo.ContainsKeyWithValue("Type", Event.KnownTypes.FeatureUsage))
                return null;

            return new SummaryData { TemplateKey = "stack-feature-summary", Data = new Dictionary<string, object>() };
        }

        public override string GetStackTitle(PersistentEvent ev) {
            if (!ShouldHandle(ev))
                return null;

            return !String.IsNullOrEmpty(ev.Source) ? ev.Source : "(Unknown)";
        }

        public override SummaryData GetEventSummaryData(PersistentEvent ev) {
            if (!ShouldHandle(ev))
                return null;

            var data = new Dictionary<string, object> { { "Source", ev.Source } };
            AddUserIdentitySummaryData(data, ev.GetUserIdentity());

            return new SummaryData { TemplateKey = "event-feature-summary", Data = data };
        }

        public override Dictionary<string, object> GetEventNotificationMailMessage(EventNotification model) {
            if (!ShouldHandle(model.Event))
                return null;

            return new Dictionary<string, object> {
                { "Subject", String.Concat("Feature: ", model.Event.Source.Truncate(120)) },
                { "BaseUrl", Settings.Current.BaseURL },
                { "Source", model.Event.Source }
            };
        }

        public override string GetEventViewName(PersistentEvent ev) {
            if (!ShouldHandle(ev))
                return null;

            return "Event-Feature";
        }
    }
}