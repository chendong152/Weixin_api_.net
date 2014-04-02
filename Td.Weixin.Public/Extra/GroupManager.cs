﻿using Newtonsoft.Json;
using Td.Weixin.Public.Common;
using Td.Weixin.Public.Extra.Models;

namespace Td.Weixin.Public.Extra
{
    public class GroupManager
    {
        public const string DefaultQueryUrl = "https://api.weixin.qq.com/cgi-bin/groups/get";

        public const string DefaultCreateUrl = "https://api.weixin.qq.com/cgi-bin/groups/create";

        public const string DefaultUpdateUrl = "https://api.weixin.qq.com/cgi-bin/groups/update";

        private GroupManager(string accessToken)
        {
            AccessToken = accessToken;
            QueryUrl = DefaultQueryUrl;
            CreateUrl = DefaultCreateUrl;
            UpdateUrl = DefaultUpdateUrl;
        }

        /// <summary>
        ///     使用缓存的accesstoken创建实例
        /// </summary>
        public static GroupManager Default
        {
            get { return new GroupManager(Credential.CachedAccessToken); }
        }

        public string AccessToken { get; set; }
        public string QueryUrl { get; set; }
        public string CreateUrl { get; set; }
        public string UpdateUrl { get; set; }

        /// <summary>
        ///     查询分组。
        ///     失败时抛出WxException异常
        /// </summary>
        /// <returns></returns>
        public WxGroupQueryResult Query()
        {
            var s = new HttpHelper(QueryUrl).GetString(new FormData
            {
                {"access_token", AccessToken}
            });
            var ret = JsonConvert.DeserializeObject<WxGroupQueryResult>(s);

            if (ret.Groups == null)
                throw new WxException(JsonConvert.DeserializeObject<BasicResult>(s));

            return ret;
        }

        /// <summary>
        ///     一个公众账号，最多支持创建500个分组。
        ///     成功后返回新组的id和名称
        ///     失败时抛出WxException异常
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public WxGroupCreateResult Create(string groupName)
        {
            var ao = new
            {
                group = new {name = groupName}
            };
            var s = new HttpHelper(CreateUrl).PostString(JsonConvert.SerializeObject(ao), new FormData
            {
                {"access_token", AccessToken}
            });

            var ret = JsonConvert.DeserializeObject<WxGroupCreateResult>(s);

            if (ret.Group == null)
                throw new WxException(JsonConvert.DeserializeObject<BasicResult>(s));

            return ret;
        }

        /// <summary>
        ///     修改分组名
        /// </summary>
        /// <param name="group">id为要修改的组的id，名称为组的新名称</param>
        /// <returns></returns>
        public BasicResult Update(WxGroup group)
        {
            var ao = new
            {
                group = new {name = group.Name, id = group.ID}
            };
            var s = new HttpHelper(UpdateUrl).PostString(JsonConvert.SerializeObject(ao), new FormData
            {
                {"access_token", AccessToken}
            });

            return JsonConvert.DeserializeObject<BasicResult>(s);
        }
    }
}