//using DataLibrary;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;

//List<AccountingAudit> accAuditList = new List<AccountingAudit>();
//List<AccountingAudit> revAccAuditList = new List<AccountingAudit>();
//List<AccountingAudit> expAccAuditList = new List<AccountingAudit>();

//int removeOrdAccsVal = new int();
//List<OrdinanceAccounting> removeOrdAccs = new List<OrdinanceAccounting>();
//if (Session["RemoveOrdAccs"] != null)
//{
//    removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
//}
//if (removeOrdAccs.Count > 0)
//{
//    foreach (OrdinanceAccounting item in removeOrdAccs)
//    {
//        item.LastUpdateBy = _user.Login;
//        item.LastUpdateDate = DateTime.Now;
//        item.EffectiveDate = DateTime.Now;
//        removeOrdAccsVal = Factory.Instance.Expire(item, "sp_UpdateOrdinance_Accounting");
//        if (removeOrdAccsVal > 0)
//        {
//            string getAmount = string.Empty;
//            if (item.Amount.ToString().Equals("-1.00") || item.Amount.ToString().Equals("-1"))
//            {
//                getAmount = $"<span>{AuditSymbol("remove")} <span data-type='String'>N/A</span></span>";
//            }
//            else
//            {
//                getAmount = $"<span>{AuditSymbol("remove")} <span data-type='Decimal'>{item.Amount}</span></span>";
//            }
//            AccountingAudit accAudit = new AccountingAudit()
//            {
//                AccountingDesc = item.AccountingDesc,
//                OrdinanceAccountingID = item.OrdinanceAccountingID,
//                FundCode = $"<span>{AuditSymbol("remove")} <span data-type='String'>{item.FundCode}</span></span>",
//                DepartmentCode = $"<span>{AuditSymbol("remove")} <span data-type='String'>{item.DepartmentCode}</span></span>",
//                UnitCode = $"<span>{AuditSymbol("remove")} <span data-type='String'>{item.UnitCode}</span></span>",
//                ActivityCode = $"<span>{AuditSymbol("remove")} <span data-type='String'>{item.ActivityCode}</span></span>",
//                ObjectCode = $"<span>{AuditSymbol("remove")} <span data-type='String'>{item.ObjectCode}</span></span>",
//                Amount = getAmount,
//            };
//            accAuditList.Add(accAudit);
//        }
//        if (removeOrdAccsVal < 1)
//        {
//            break;
//        }
//    }

//}
//else
//{
//    removeOrdAccsVal = 1;
//}

//int updateRevAccsVal = new int();
//int updateExpAccsVal = new int();
//if (removeOrdAccsVal > 0)
//{
//    if (rpRevenueTable.Items.Count > 0)
//    {
//        for (int i = 0; i < rpRevenueTable.Items.Count; i++)
//        {
//            OrdinanceAccounting accountingItem = GetAccountingItem("revenue", i);
//            if (accountingItem.OrdinanceAccountingID > 0)
//            {
//                updateRevAccsVal = Factory.Instance.Update(accountingItem, "sp_UpdateOrdinance_Accounting");

//                List<OrdinanceAccounting> originalRevList = Session["OriginalRevTable"] as List<OrdinanceAccounting>;

//                OrdinanceAccounting originalRevItem = originalRevList.First(r => r.OrdinanceAccountingID.Equals(accountingItem.OrdinanceAccountingID));

//                PropertyInfo[] properties = typeof(OrdinanceAccounting).GetProperties();

//                if (revAccAuditList.Count > 0 || (properties.Any(p => !p.GetValue(accountingItem).Equals(p.GetValue(originalRevItem)) && !baseData.Any(b => b.Contains(p.Name)))))
//                {
//                    AccountingAudit accAudit = new AccountingAudit()
//                    {
//                        AccountingDesc = accountingItem.AccountingDesc,
//                        OrdinanceAccountingID = accountingItem.OrdinanceAccountingID,
//                    };
//                    foreach (PropertyInfo property in properties.Where(p => !baseData.Any(b => b.Contains(p.Name))))
//                    {
//                        if (!property.GetValue(originalRevItem).Equals(property.GetValue(accountingItem)))
//                        {
//                            switch (property.Name)
//                            {
//                                case "FundCode":
//                                    accAudit.FundCode = $"<span><span data-type='String'>{originalRevItem.FundCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.FundCode}</span></span>";
//                                    break;
//                                case "DepartmentCode":
//                                    accAudit.DepartmentCode = $"<span><span data-type='String'>{originalRevItem.DepartmentCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.DepartmentCode}</span></span>";
//                                    break;
//                                case "UnitCode":
//                                    accAudit.UnitCode = $"<span><span data-type='String'>{originalRevItem.UnitCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.UnitCode}</span></span>";
//                                    break;
//                                case "ActivityCode":
//                                    accAudit.ActivityCode = $"<span><span data-type='String'>{originalRevItem.ActivityCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.ActivityCode}</span></span>";
//                                    break;
//                                case "ObjectCode":
//                                    accAudit.ObjectCode = $"<span><span data-type='String'>{originalRevItem.ObjectCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.ObjectCode}</span></span>";
//                                    break;
//                                case "Amount":
//                                    if (originalRevItem.Amount.ToString().Equals("-1.00") || originalRevItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span>{AuditSymbol("add")} <span data-type='Decimal'>{accountingItem.Amount}</span></span>";
//                                    }
//                                    else if (accountingItem.Amount.ToString().Equals("-1.00") || accountingItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span>{AuditSymbol("remove")} <span data-type='Decimal'>{originalRevItem.Amount}</span></span>";
//                                    }
//                                    else
//                                    {
//                                        accAudit.Amount = $"<span><span data-type='Decimal'>{originalRevItem.Amount}</span> {AuditSymbol("update")} <span data-type='Decimal'>{accountingItem.Amount}</span></span>";
//                                    }
//                                    break;
//                            }
//                        }
//                        else
//                        {
//                            switch (property.Name)
//                            {
//                                case "FundCode":
//                                    accAudit.FundCode = $"<span data-type='String'>{accountingItem.FundCode}</span>";
//                                    break;
//                                case "DepartmentCode":
//                                    accAudit.DepartmentCode = $"<span data-type='String'>{accountingItem.DepartmentCode}</span>";
//                                    break;
//                                case "UnitCode":
//                                    accAudit.UnitCode = $"<span data-type='String'>{accountingItem.UnitCode}</span>";
//                                    break;
//                                case "ActivityCode":
//                                    accAudit.ActivityCode = $"<span data-type='String'>{accountingItem.ActivityCode}</span>";
//                                    break;
//                                case "ObjectCode":
//                                    accAudit.ObjectCode = $"<span data-type='String'>{accountingItem.ObjectCode}</span>";
//                                    break;
//                                case "Amount":
//                                    if (accountingItem.Amount.ToString().Equals("-1.00") || accountingItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span data-type='String'>N/A</span>";
//                                    }
//                                    else
//                                    {
//                                        accAudit.Amount = $"<span data-type='Decimal'>{accountingItem.Amount}</span>";
//                                    }
//                                    break;
//                            }
//                        }
//                    }
//                    revAccAuditList.Add(accAudit);
//                }
//                else
//                {
//                    AccountingAudit accAudit = new AccountingAudit()
//                    {
//                        AccountingDesc = accountingItem.AccountingDesc,
//                        OrdinanceAccountingID = accountingItem.OrdinanceAccountingID,
//                    };
//                    foreach (PropertyInfo property in properties.Where(p => !baseData.Any(b => b.Contains(p.Name))))
//                    {
//                        if (!property.GetValue(originalRevItem).Equals(property.GetValue(accountingItem)))
//                        {
//                            switch (property.Name)
//                            {
//                                case "FundCode":
//                                    accAudit.FundCode = $"<span><span data-type='String'>{originalRevItem.FundCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.FundCode}</span></span>";
//                                    break;
//                                case "DepartmentCode":
//                                    accAudit.DepartmentCode = $"<span><span data-type='String'>{originalRevItem.DepartmentCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.DepartmentCode}</span></span>";
//                                    break;
//                                case "UnitCode":
//                                    accAudit.UnitCode = $"<span><span data-type='String'>{originalRevItem.UnitCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.UnitCode}</span></span>";
//                                    break;
//                                case "ActivityCode":
//                                    accAudit.ActivityCode = $"<span><span data-type='String'>{originalRevItem.ActivityCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.ActivityCode}</span></span>";
//                                    break;
//                                case "ObjectCode":
//                                    accAudit.ObjectCode = $"<span><span data-type='String'>{originalRevItem.ObjectCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.ObjectCode}</span></span>";
//                                    break;
//                                case "Amount":
//                                    if (originalRevItem.Amount.ToString().Equals("-1.00") || originalRevItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span>{AuditSymbol("add")} <span data-type='Decimal'>{accountingItem.Amount}</span></span>";
//                                    }
//                                    else if (accountingItem.Amount.ToString().Equals("-1.00") || accountingItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span>{AuditSymbol("remove")} <span data-type='Decimal'>{originalRevItem.Amount}</span></span>";
//                                    }
//                                    else
//                                    {
//                                        accAudit.Amount = $"<span><span data-type='Decimal'>{originalRevItem.Amount}</span> {AuditSymbol("update")} <span data-type='Decimal'>{accountingItem.Amount}</span></span>";
//                                    }
//                                    break;
//                            }
//                        }
//                        else
//                        {
//                            switch (property.Name)
//                            {
//                                case "FundCode":
//                                    accAudit.FundCode = $"<span data-type='String'>{accountingItem.FundCode}</span>";
//                                    break;
//                                case "DepartmentCode":
//                                    accAudit.DepartmentCode = $"<span data-type='String'>{accountingItem.DepartmentCode}</span>";
//                                    break;
//                                case "UnitCode":
//                                    accAudit.UnitCode = $"<span data-type='String'>{accountingItem.UnitCode}</span>";
//                                    break;
//                                case "ActivityCode":
//                                    accAudit.ActivityCode = $"<span data-type='String'>{accountingItem.ActivityCode}</span>";
//                                    break;
//                                case "ObjectCode":
//                                    accAudit.ObjectCode = $"<span data-type='String'>{accountingItem.ObjectCode}</span>";
//                                    break;
//                                case "Amount":
//                                    if (accountingItem.Amount.ToString().Equals("-1.00") || accountingItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span data-type='String'>N/A</span>";
//                                    }
//                                    else
//                                    {
//                                        accAudit.Amount = $"<span data-type='Decimal'>{accountingItem.Amount}</span>";
//                                    }
//                                    break;
//                            }
//                        }
//                    }
//                    revAccAuditList.Add(accAudit);
//                }
//            }
//            else
//            {
//                updateRevAccsVal = Factory.Instance.Insert(accountingItem, "sp_InsertOrdinance_Accounting", Skips("accountingInsert"));
//                string getAmount = string.Empty;
//                if (accountingItem.Amount.ToString().Equals("-1.00") || accountingItem.Amount.ToString().Equals("-1"))
//                {
//                    getAmount = $"<span>{AuditSymbol("add")} <span data-type='String'>N/A</span></span>";
//                }
//                else
//                {
//                    getAmount = $"<span>{AuditSymbol("add")} <span data-type='Decimal'>{accountingItem.Amount}</span></span>";
//                }
//                if (updateRevAccsVal > 0)
//                {
//                    AccountingAudit accAudit = new AccountingAudit()
//                    {
//                        AccountingDesc = accountingItem.AccountingDesc,
//                        OrdinanceAccountingID = updateRevAccsVal,
//                        FundCode = $"<span>{AuditSymbol("add")} <span data-type='String'>{accountingItem.FundCode}</span></span>",
//                        DepartmentCode = $"<span>{AuditSymbol("add")} <span data-type='String'>{accountingItem.DepartmentCode}</span></span>",
//                        UnitCode = $"<span>{AuditSymbol("add")} <span data-type='String'>{accountingItem.UnitCode}</span></span>",
//                        ActivityCode = $"<span>{AuditSymbol("add")} <span data-type='String'>{accountingItem.ActivityCode}</span></span>",
//                        ObjectCode = $"<span>{AuditSymbol("add")} <span data-type='String'>{accountingItem.ObjectCode}</span></span>",
//                        Amount = getAmount,
//                    };
//                    revAccAuditList.Add(accAudit);
//                }
//                else
//                {
//                    updateRevAccsVal = 0;
//                    break;
//                }
//            }
//            if (updateRevAccsVal < 1)
//            {
//                break;
//            }
//        }
//    }
//    else
//    {
//        updateRevAccsVal = 1;
//    }

//    if (rpExpenditureTable.Items.Count > 0)
//    {
//        List<OrdinanceAccounting> originalExpList = Session["OriginalExpTable"] as List<OrdinanceAccounting>;
//        PropertyInfo[] properties = typeof(OrdinanceAccounting).GetProperties();
//        for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
//        {
//            OrdinanceAccounting accountingItem = GetAccountingItem("expenditure", i);
//            if (accountingItem.OrdinanceAccountingID > 0 && originalExpList.Any(oe => properties.Any(pr => !pr.GetValue(oe).Equals(pr.GetValue(accountingItem)))))
//            {
//                updateExpAccsVal = Factory.Instance.Update(accountingItem, "sp_UpdateOrdinance_Accounting");

//                OrdinanceAccounting originalExpItem = originalExpList.First(r => r.OrdinanceAccountingID.Equals(accountingItem.OrdinanceAccountingID));

//                if (expAccAuditList.Count > 0 || (properties.Any(p => !p.GetValue(accountingItem).Equals(p.GetValue(originalExpItem)) && !baseData.Any(b => b.Contains(p.Name)))))
//                {
//                    AccountingAudit accAudit = new AccountingAudit()
//                    {
//                        AccountingDesc = accountingItem.AccountingDesc,
//                        OrdinanceAccountingID = accountingItem.OrdinanceAccountingID,
//                    };
//                    foreach (PropertyInfo property in properties.Where(p => !baseData.Any(b => b.Contains(p.Name))))
//                    {
//                        if (!property.GetValue(originalExpItem).Equals(property.GetValue(accountingItem)))
//                        {
//                            switch (property.Name)
//                            {
//                                case "FundCode":
//                                    accAudit.FundCode = $"<span><span data-type='String'>{originalExpItem.FundCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.FundCode}</span></span>";
//                                    break;
//                                case "DepartmentCode":
//                                    accAudit.DepartmentCode = $"<span><span data-type='String'>{originalExpItem.DepartmentCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.DepartmentCode}</span></span>";
//                                    break;
//                                case "UnitCode":
//                                    accAudit.UnitCode = $"<span><span data-type='String'>{originalExpItem.UnitCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.UnitCode}</span></span>";
//                                    break;
//                                case "ActivityCode":
//                                    accAudit.ActivityCode = $"<span><span data-type='String'>{originalExpItem.ActivityCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.ActivityCode}</span></span>";
//                                    break;
//                                case "ObjectCode":
//                                    accAudit.ObjectCode = $"<span><span data-type='String'>{originalExpItem.ObjectCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.ObjectCode}</span></span>";
//                                    break;
//                                case "Amount":
//                                    if (originalExpItem.Amount.ToString().Equals("-1.00") || originalExpItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span>{AuditSymbol("add")} <span data-type='Decimal'>{accountingItem.Amount}</span></span>";
//                                    }
//                                    else if (accountingItem.Amount.ToString().Equals("-1.00") || accountingItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span>{AuditSymbol("remove")} <span data-type='Decimal'>{originalExpItem.Amount}</span></span>";
//                                    }
//                                    else
//                                    {
//                                        accAudit.Amount = $"<span><span data-type='Decimal'>{originalExpItem.Amount}</span> {AuditSymbol("update")} <span data-type='Decimal'>{accountingItem.Amount}</span></span>";
//                                    }
//                                    break;
//                            }
//                        }
//                        else
//                        {
//                            switch (property.Name)
//                            {
//                                case "FundCode":
//                                    accAudit.FundCode = $"<span data-type='String'>{accountingItem.FundCode}</span>";
//                                    break;
//                                case "DepartmentCode":
//                                    accAudit.DepartmentCode = $"<span data-type='String'>{accountingItem.DepartmentCode}</span>";
//                                    break;
//                                case "UnitCode":
//                                    accAudit.UnitCode = $"<span data-type='String'>{accountingItem.UnitCode}</span>";
//                                    break;
//                                case "ActivityCode":
//                                    accAudit.ActivityCode = $"<span data-type='String'>{accountingItem.ActivityCode}</span>";
//                                    break;
//                                case "ObjectCode":
//                                    accAudit.ObjectCode = $"<span data-type='String'>{accountingItem.ObjectCode}</span>";
//                                    break;
//                                case "Amount":
//                                    if (accountingItem.Amount.ToString().Equals("-1.00") || accountingItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span data-type='String'>N/A</span>";
//                                    }
//                                    else
//                                    {
//                                        accAudit.Amount = $"<span data-type='Decimal'>{accountingItem.Amount}</span>";
//                                    }
//                                    break;
//                            }
//                        }
//                    }
//                    expAccAuditList.Add(accAudit);
//                }
//                else
//                {
//                    AccountingAudit accAudit = new AccountingAudit()
//                    {
//                        AccountingDesc = accountingItem.AccountingDesc,
//                        OrdinanceAccountingID = accountingItem.OrdinanceAccountingID,
//                    };
//                    foreach (PropertyInfo property in properties.Where(p => !baseData.Any(b => b.Contains(p.Name))))
//                    {
//                        if (!property.GetValue(originalExpItem).Equals(property.GetValue(accountingItem)))
//                        {
//                            switch (property.Name)
//                            {
//                                case "FundCode":
//                                    accAudit.FundCode = $"<span><span data-type='String'>{originalExpItem.FundCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.FundCode}</span></span>";
//                                    break;
//                                case "DepartmentCode":
//                                    accAudit.DepartmentCode = $"<span><span data-type='String'>{originalExpItem.DepartmentCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.DepartmentCode}</span></span>";
//                                    break;
//                                case "UnitCode":
//                                    accAudit.UnitCode = $"<span><span data-type='String'>{originalExpItem.UnitCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.UnitCode}</span></span>";
//                                    break;
//                                case "ActivityCode":
//                                    accAudit.ActivityCode = $"<span><span data-type='String'>{originalExpItem.ActivityCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.ActivityCode}</span></span>";
//                                    break;
//                                case "ObjectCode":
//                                    accAudit.ObjectCode = $"<span><span data-type='String'>{originalExpItem.ObjectCode}</span> {AuditSymbol("update")} <span data-type='String'>{accountingItem.ObjectCode}</span></span>";
//                                    break;
//                                case "Amount":
//                                    if (originalExpItem.Amount.ToString().Equals("-1.00") || originalExpItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span>{AuditSymbol("add")} <span data-type='Decimal'>{accountingItem.Amount}</span></span>";
//                                    }
//                                    else if (accountingItem.Amount.ToString().Equals("-1.00") || accountingItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span>{AuditSymbol("remove")} <span data-type='Decimal'>{originalExpItem.Amount}</span></span>";
//                                    }
//                                    else
//                                    {
//                                        accAudit.Amount = $"<span><span data-type='Decimal'>{originalExpItem.Amount}</span> {AuditSymbol("update")} <span data-type='Decimal'>{accountingItem.Amount}</span></span>";
//                                    }
//                                    break;
//                            }
//                        }
//                        else
//                        {
//                            switch (property.Name)
//                            {
//                                case "FundCode":
//                                    accAudit.FundCode = $"<span data-type='String'>{accountingItem.FundCode}</span>";
//                                    break;
//                                case "DepartmentCode":
//                                    accAudit.DepartmentCode = $"<span data-type='String'>{accountingItem.DepartmentCode}</span>";
//                                    break;
//                                case "UnitCode":
//                                    accAudit.UnitCode = $"<span data-type='String'>{accountingItem.UnitCode}</span>";
//                                    break;
//                                case "ActivityCode":
//                                    accAudit.ActivityCode = $"<span data-type='String'>{accountingItem.ActivityCode}</span>";
//                                    break;
//                                case "ObjectCode":
//                                    accAudit.ObjectCode = $"<span data-type='String'>{accountingItem.ObjectCode}</span>";
//                                    break;
//                                case "Amount":
//                                    if (accountingItem.Amount.ToString().Equals("-1.00") || accountingItem.Amount.ToString().Equals("-1"))
//                                    {
//                                        accAudit.Amount = $"<span data-type='String'>N/A</span>";
//                                    }
//                                    else
//                                    {
//                                        accAudit.Amount = $"<span data-type='Decimal'>{accountingItem.Amount}</span>";
//                                    }
//                                    break;
//                            }
//                        }
//                    }
//                    expAccAuditList.Add(accAudit);
//                }
//            }
//            else
//            {
//                updateExpAccsVal = Factory.Instance.Insert(accountingItem, "sp_InsertOrdinance_Accounting", Skips("accountingInsert"));
//                string getAmount = string.Empty;
//                if (accountingItem.Amount.ToString().Equals("-1.00") || accountingItem.Amount.ToString().Equals("-1"))
//                {
//                    getAmount = $"<span>{AuditSymbol("add")} <span data-type='String'>N/A</span></span>";
//                }
//                else
//                {
//                    getAmount = $"<span>{AuditSymbol("add")} <span data-type='Decimal'>{accountingItem.Amount}</span></span>";
//                }
//                if (updateExpAccsVal > 0)
//                {
//                    AccountingAudit accAudit = new AccountingAudit()
//                    {
//                        AccountingDesc = accountingItem.AccountingDesc,
//                        OrdinanceAccountingID = updateExpAccsVal,
//                        FundCode = $"<span>{AuditSymbol("add")} <span data-type='String'>{accountingItem.FundCode}</span></span>",
//                        DepartmentCode = $"<span>{AuditSymbol("add")} <span data-type='String'>{accountingItem.DepartmentCode}</span></span>",
//                        UnitCode = $"<span>{AuditSymbol("add")} <span data-type='String'>{accountingItem.UnitCode}</span></span>",
//                        ActivityCode = $"<span>{AuditSymbol("add")} <span data-type='String'>{accountingItem.ActivityCode}</span></span>",
//                        ObjectCode = $"<span>{AuditSymbol("add")} <span data-type='String'>{accountingItem.ObjectCode}</span></span>",
//                        Amount = getAmount,
//                    };
//                    expAccAuditList.Add(accAudit);
//                }
//                else
//                {
//                    updateExpAccsVal = 0;
//                    break;
//                }
//            }
//            if (updateExpAccsVal < 1)
//            {
//                break;
//            }
//        }
//    }
//    else
//    {
//        updateExpAccsVal = 1;
//    }

//    if (revAccAuditList.Count > 0)
//    {
//        accAuditList.AddRange(revAccAuditList);
//    }
//    if (expAccAuditList.Count > 0)
//    {
//        accAuditList.AddRange(expAccAuditList);
//    }
//}