using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using HosPro.Data;
using HosPro.Models;
using Newtonsoft.Json;

namespace HosPro.Controllers
{
    public class HomeController : Controller
    {

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetPatList(PatSearchModel searchInput)
        {

            using (var db = new dbEntities()) //创建数据库上下文
            {
                var list = db.Sys_HosData.AsQueryable();
                #region 过滤条件
                list = list.Where(s => s.F_HospitalId.ToString() == searchInput.HospitalId && s.F_TelPhone.Length == 11);//过滤医院和电话
                if (!string.IsNullOrEmpty(searchInput.DeptName))
                    list = list.Where(s => s.F_DeptName == searchInput.DeptName);//过滤科室
                if (searchInput.Type == "1")
                    list = list.Where(s => s.F_Insurance.Contains(searchInput.Type));//过滤类型
                #region 年龄范围
                if (searchInput.AgeRange != 0)
                {
                    int minAge = 0, maxAge = 0;
                    switch (searchInput.AgeRange)
                    {
                        case 1:
                            minAge = 0;
                            maxAge = 19;
                            break;
                        case 7:
                            minAge = 70;
                            maxAge = 120;
                            break;
                        default:
                            minAge = searchInput.AgeRange * 10;
                            maxAge = searchInput.AgeRange * 10 + 9;
                            break;
                    }
                    list = list.Where(s => s.F_Age >= minAge && s.F_Age <= maxAge);//年龄过滤

                }
                #endregion

                #region 呼叫过滤
                if (!string.IsNullOrEmpty(searchInput.CallType))
                {
                    //已呼
                    if (searchInput.CallType == "1")
                        list = list.Where(s => s.F_CallNum > 0);
                    else
                        list = list.Where(s => s.F_CallNum == 0);
                }
                #endregion

                #endregion
                var count = list.Count();
                var rows = list.OrderByDescending(t => t.F_AdmissionTime).Skip((searchInput.PageIndex - 1) * searchInput.PageSize).Take(searchInput.PageSize);//分页数据
                string json = JsonConvert.SerializeObject(new { count = count, rows = rows });
                return Content(json);
            }

            //string sql = $"SELECT * FROM SYS_HosData where F_HospitalId = '{searchInput.HoispitalId}' and len(F_TelPhone)=11";
            //if (!string.IsNullOrEmpty(searchInput.DeptName))
            //{
            //    sql += $" and F_DeptName = '{searchInput.DeptName}'";
            //}
            //if (searchInput.Type == "1")
            //{
            //    sql += " and F_Insurance like '%商%'";
            //}

            //using (var db = new dbEntities()) //创建数据库上下文
            //{
            //    var data = db.Database.SqlQuery<Sys_HosData>(sql);
            //    string json = JsonConvert.SerializeObject(data);
            //    return Content(json);
            //}
        }

        public ActionResult GetDeptList(string hospitalId)
        {
            string sql = $"SELECT F_DeptName as DeptName FROM SYS_HosData where F_HospitalId = '{hospitalId}' group by F_DeptName";
            using (var db = new dbEntities()) //创建数据库上下文
            {
                var data = db.Database.SqlQuery<DeptModel>(sql);
                string json = JsonConvert.SerializeObject(data);
                return Content(json);
            }
        }

        public ActionResult UpdateCallNum(string id, string hospitalId, string tel, string name, string wechat, string remark)
        {
            var log = new Sys_CallLog() { F_HospitalId = Convert.ToInt32(hospitalId), F_Name = name, F_Tel = tel, F_Time = DateTime.Now };
            string sql = $"update SYS_HosData set F_CallNum= F_CallNum+1,F_WeChatFlag={wechat},F_Remark='{remark}' where F_Id = '{id}'";
            using (var db = new dbEntities()) //创建数据库上下文
            {
                //var trans = db.Database.BeginTransaction();
                var data = db.Database.ExecuteSqlCommand(sql);
                db.Sys_CallLog.Add(log);
                db.SaveChanges();
                return Content(JsonConvert.SerializeObject(new { result = data }));
            }
        }

    }

    #region InputModel

    public class PatSearchModel
    {
        /// <summary>
        /// 医院ID
        /// </summary>
        public string HospitalId { get; set; }

        /// <summary>
        /// 科室名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 年龄范围
        /// </summary>
        public int AgeRange { get; set; }

        /// <summary>
        /// 呼叫类型
        /// </summary>
        public string CallType { get; set; }

        /// <summary>
        /// 页索引
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; }
    }


    #endregion
}