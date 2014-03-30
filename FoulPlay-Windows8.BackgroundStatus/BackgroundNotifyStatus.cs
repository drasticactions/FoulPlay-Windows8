using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Foulplay_Windows8.Core.Entities;
using Foulplay_Windows8.Core.Managers;
using Foulplay_Windows8.Core.Tools;

namespace FoulPlay_Windows8.BackgroundStatus
{
    public sealed class BackgroundNotifyStatus : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            if (NotifyStatusTile.IsInternet())
            {
                await Update(taskInstance);
            }
            deferral.Complete();
        }

        private async Task Update(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                var userAccountEntity = new UserAccountEntity();
                var authManager = new AuthenticationManager();
                bool loginTest = await authManager.RefreshAccessToken(userAccountEntity);
                if (loginTest)
                {
                    UserAccountEntity.User user = await authManager.GetUserEntity(userAccountEntity);
                    if (user == null) return;
                    userAccountEntity.SetUserEntity(user);
                    NotificationEntity notificationEntity = await GetNotifications(userAccountEntity);
                    if (notificationEntity == null) return;
                    if (notificationEntity.Notifications == null) return;

                    // Debug
                    //NotifyStatusTile.CreateNotificationLiveTile(notificationEntity.Notifications.First());
                    //NotifyStatusTile.CreateToastNotification(notificationEntity.Notifications.First());

                    var notificationList = notificationEntity.Notifications.Where(o => o.SeenFlag == false);
                    foreach (var notification in notificationList)
                    {
                        NotifyStatusTile.CreateNotificationLiveTile(notification);
                        NotifyStatusTile.CreateToastNotification(notification);
                        await NotificationManager.ClearNotification(notification, userAccountEntity);
                    }
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to show toast/live tile notification");
            }
        }

        private async Task<NotificationEntity> GetNotifications(UserAccountEntity userAccountEntity)
        {
            var notificationManager = new NotificationManager();
            return await notificationManager.GetNotifications(userAccountEntity.GetUserEntity().OnlineId, userAccountEntity);
        }
    }
}
