﻿using BL.Casting;
using BL.Helpers;
using DAL;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    /*public enum EStatus
    {
        NotFound,
        Found ,
        TimeWait,
        TimeOver
    }*/


    public class Searches
    {
        public static bool isBeforeToday(DateTime d)
        {
            return d.Year <= DateTime.Today.Year && d.Month <= DateTime.Today.Month && d.Day < DateTime.Today.Day;
        }

        public static bool isAfterToday(DateTime d)
        {
            return d.Year >= DateTime.Today.Year && d.Month >= DateTime.Today.Month && d.Day > DateTime.Today.Day;
        }
        public static EStatus CheckStatus(SearchDTO search)
        {
            if (search.status == EStatus.Found || search.status == EStatus.Deleted)
                return search.status;
            if (isBeforeToday((DateTime)search.dateEnd))
            {
                return EStatus.TimeOver;
            }
            if (isAfterToday((DateTime)search.dateStart))
                return EStatus.TimeWait;
            return EStatus.NotFound;
        }
        //Returns the categories for choosing
        public static WebResult<List<CategoryDTO>> GetCategories()
        {
            using (ProjectEntities db = new ProjectEntities())
            {

                return new WebResult<List<CategoryDTO>>
                {
                    Message = "רשימת הקטגוריות נשלחה בהצלחה",
                    Status = true,
                    Value = CategoryCast.GetCategoriesDTO(db.Categories.ToList())
                };
            }
        }
        //Create search
        public static WebResult<SearchDTO> Create(SearchDTO searchDTO, string passwordUser)
        {
            using (ProjectEntities db = new ProjectEntities())
            {
                searchDTO.status = CheckStatus(searchDTO);

                try
                {
                    searchDTO.codeUser = db.Users.FirstOrDefault(f => f.passwordUser == passwordUser).codeUser;
                    db.Searches.Add(SearchCast.GetSearch(searchDTO));
                    db.SaveChanges();
                    return new WebResult<SearchDTO>
                    {
                        Message = "יצירת חיפוש בוצעה בהצלחה",
                        Status = true,
                        Value = searchDTO
                    };
                }
                catch (Exception e)
                {
                    return new WebResult<SearchDTO>()
                    {
                        Message = e.Message,
                        Status = false,
                        Value = null
                    };
                }

            }
        }
        //Delete search- status changes to 2
        public static WebResult<SearchDTO> Delete(int code)
        {
            using (ProjectEntities db = new ProjectEntities())
            {

                Search search =db.Searches.Find(code);
                if (search == null)
                    return new WebResult<SearchDTO>
                    {
                        Message = "לא נמצא חיפוש זה",
                        Status = false,
                        Value = null
                    };
                search.status = (int) EStatus.Deleted;
                db.SaveChanges();
                return new WebResult<SearchDTO>
                {
                    Message = "המחיקה בוצעה בהצלחה",
                    Status = true,
                    Value = SearchCast.GetSearchDTO(search)
                };
            };
        }

        //Search is found- user bought the product
        public static WebResult<SearchDTO> Found(int codeSearch, string mailShop)
        {
            using (ProjectEntities db = new ProjectEntities())
            {

                Search search = db.Searches.Find(codeSearch);
                if (search == null)
                    return new WebResult<SearchDTO>
                    {
                        Message = "לא נמצא חיפוש זה במאגר",
                        Status = false,
                        Value = null
                    };
                search.status =(int) EStatus.Found;
                search.codeShop = db.Shops.FirstOrDefault(f => f.mailShop == mailShop).codeShop;
                db.SaveChanges();

                return new WebResult<SearchDTO>
                {
                    Message = "החיפוש נמצא בהצלחה",
                    Status = true,
                    Value =SearchCast.GetSearchDTO(search)
                };
            }
        }

        public static WebResult<List<SearchDTO>> UpdateAllSearchStatus()
        {
            using (ProjectEntities db = new ProjectEntities())
            {

                foreach (var s in db.Searches)
                {
                    UpdateSearchStatus(s.codeSearch, CheckStatus(SearchCast.GetSearchDTO(s)), null);
                }
                db.SaveChanges();
                return new WebResult<List<SearchDTO>>
                {
                    Message = "רשימת מטלות עודכנה בהצלחה",
                    Status = true,
                    Value = SearchCast.GetSearchesDTO(db.Searches.ToList())
                };
            }
        }

        //Returns history of the searches, even thouse the user found
        public static WebResult<List<SearchDetailsForUser>> GetHistory(string passwordUser)
        {
            using (ProjectEntities db = new ProjectEntities())
            {
                string pass = passwordUser;
                User CurrentUser = db.Users.FirstOrDefault(f => f.passwordUser == pass);
                if (CurrentUser == null)
                {
                    return new WebResult<List<SearchDetailsForUser>>
                    {
                        Message = "the user cant find!",
                        Value = null,
                        Status = false
                    };
                }
                List<SearchDetailsForUser> searchesForUser = new List<SearchDetailsForUser>();
                var searches = SearchCast.GetSearchesDTO(db.Searches.ToList());
                foreach (var search in searches)
                {
                    if (search.codeUser == CurrentUser.codeUser && search.status != EStatus.Deleted)
                    {
                        searchesForUser.Add(new SearchDetailsForUser()
                        {
                            codeSearch = search.codeSearch,
                            nameProduct = search.nameProduct,
                            nameCategory = db.Categories.First(f => f.codeCategory == search.codeCategory).nameCategory,
                            status = search.status,
                            nameShop = search.codeShop == null ? "" : db.Shops.First(f => f.codeShop == search.codeShop).nameShop
                        });
                    }

                }
                searchesForUser.Reverse();
                return new WebResult<List<SearchDetailsForUser>>
                {
                    Message = "חיפושי המשתמש נשלחו בהצלחה",
                    Value = searchesForUser,
                    Status = true
                };
            }
        }
        //Returns user searches that have not yet been found
        public static WebResult<List<SearchDetailsForUser>> GetHistoryNotFound(string passwordUser)
        {
            using (ProjectEntities db = new ProjectEntities())
            {

                User CurrentUser = db.Users.FirstOrDefault(f => f.passwordUser == passwordUser);
                List<SearchDetailsForUser> searchesForUser = new List<SearchDetailsForUser>();

                var searches = SearchCast.GetSearchesDTO(db.Searches.ToList());
                foreach (var search in searches)
                {
                    if (search.codeUser == CurrentUser.codeUser && search.status == 0)
                    {
                        searchesForUser.Add(new SearchDetailsForUser()
                        {
                            codeSearch = search.codeSearch,
                            nameProduct = search.nameProduct,
                            nameCategory = db.Categories.First(f => f.codeCategory == search.codeCategory).nameCategory,
                            status = search.status
                        });
                    }
                }
                searchesForUser.Reverse();
                return new WebResult<List<SearchDetailsForUser>>
                {
                    Message = "חיפושי המשתמש נשלחו בהצלחה",
                    Value = searchesForUser,
                    Status = true
                };
            }
        }
        //Returns user searches that have been found
        public static WebResult<List<SearchDetailsForUser>> GetHistoryFound(string passwordUser)
        {
            using (ProjectEntities db = new ProjectEntities())
            {

                User CurrentUser = db.Users.FirstOrDefault(f => f.passwordUser == passwordUser);
                List<SearchDetailsForUser> searchesForUser = new List<SearchDetailsForUser>();
                var searches = SearchCast.GetSearchesDTO(db.Searches.ToList());
                foreach (var search in searches)
                {
                    if (search.codeUser == CurrentUser.codeUser && search.status == EStatus.Found)
                    {
                        searchesForUser.Add(new SearchDetailsForUser()
                        {
                            codeSearch = search.codeSearch,
                            nameProduct = search.nameProduct,
                            nameCategory = db.Categories.First(f => f.codeCategory == search.codeCategory).nameCategory,
                            status = search.status,
                            nameShop = search.codeShop == null ? "" : db.Shops.First(f => f.codeShop == search.codeShop).nameShop
                        });
                    }
                }
                searchesForUser.Reverse();
                return new WebResult<List<SearchDetailsForUser>>
                {
                    Message = "חיפושי המשתמש נשלחו בהצלחה",
                    Value = searchesForUser,
                    Status = true
                };
            }
        }
        //Returns all stores that sell a particular category
        public static WebResult<List<ShopDetailsForUsers>> GetShopsForCategory(int codeCategory)
        {
            using (ProjectEntities db = new ProjectEntities())
            {

                List<int> codeShops = new List<int>();
                Shop shop;
                List<ShopDetailsForUsers> shopsToCategory = new List<ShopDetailsForUsers>();
                codeShops = db.Category_to_shop.Where(w => w.codeCategory == codeCategory).Select(s => s.codeShop).ToList();
                if (codeShops.Count == 0)
                    return new WebResult<List<ShopDetailsForUsers>>()
                    {
                        Status = false,
                        Message = "לא נמצאה חנות שמוכרת קטגוריה זו",
                        Value = null
                    };
                foreach (var code in codeShops)
                {
                    shop = db.Shops.Find(code);
                    shopsToCategory.Add(new ShopDetailsForUsers()
                    {
                        NameShop = shop.nameShop,
                        AddressString = shop.addressString,
                        FromHour = shop.fromHour,
                        ToHour = shop.toHour,
                        Latitude = shop.latitude,
                        Longitude = shop.longitude,
                        PhoneShop = shop.phoneShop
                    });
                }
                return new WebResult<List<ShopDetailsForUsers>>()
                {
                    Status = true,
                    Message = "להלן החנויות בהתאם לקטגוריה",
                    Value = shopsToCategory
                };
            }
        }
        public static WebResult<List<SearchDetailsForUser>> SearchByStatus(string passwordUser, EStatus status)
        {
            using (ProjectEntities db = new ProjectEntities())
            {

                User CurrentUser = db.Users.FirstOrDefault(f => f.passwordUser == passwordUser);
                List<SearchDetailsForUser> searchesForUser = new List<SearchDetailsForUser>();
                var searches = SearchCast.GetSearchesDTO(db.Searches.ToList());
                foreach (var search in searches)
                {
                    if (search.codeUser == CurrentUser.codeUser && search.status == status)
                    {
                        searchesForUser.Add(new SearchDetailsForUser()
                        {
                            codeSearch = search.codeSearch,
                            nameProduct = search.nameProduct,
                            nameCategory = db.Categories.First(f => f.codeCategory == search.codeCategory).nameCategory,
                            status = search.status,
                            nameShop = search.codeShop == null ? "" : db.Shops.First(f => f.codeShop == search.codeShop).nameShop
                        });
                    }
                }
                searchesForUser.Reverse();
                return new WebResult<List<SearchDetailsForUser>>
                {
                    Message = "חיפושי המשתמש נשלחו בהצלחה",
                    Value = searchesForUser,
                    Status = true
                };
            }
        }
        public static Search UpdateSearchStatus(int codeSearch, EStatus status, string mailShop)
        {
            using (ProjectEntities db = new ProjectEntities())
            {

                Search search = db.Searches.Find(codeSearch);
                if (search == null)
                    return null;
                search.status =(int) status;
                //if (status == EStatus.Found)
                //    search.codeShop = db.Shops.FirstOrDefault(f => f.mailShop == mailShop).codeShop;
                db.SaveChanges();

                return search;
            }

            //public static WebResult<SearchDTO> UpdateSearchStatus(int code, EStatus eStatus, string shop)
            //{

            //    using (ProjectEntities db = new ProjectEntities())
            //    {

            //        SearchDTO search = SearchCast.GetSearchDTO(db.Searches.Find(code));
            //        if (search == null)
            //            return new WebResult<SearchDTO>
            //            {
            //                Message = "לא נמצא חיפוש זה במאגר",
            //                Status = false,
            //                Value = null
            //            };
            //        search.status = status;
            //        if (status == EStatus.Found)
            //            search.codeShop = db.Shops.FirstOrDefault(f => f.mailShop == shop).codeShop;
            //        db.SaveChanges();

            //        return new WebResult<SearchDTO>
            //        {
            //            Message = "החיפוש נמצא בהצלחה",
            //            Status = true,
            //            Value = search
            //        };
            //    }
            //}

            //public static WebResult<List<SearchDTO>> UpdateAllSearchStatus()
            //{
            //    using (ProjectEntities db = new ProjectEntities())
            //    {

            //        SearchCast.GetSearchesDTO(db.Searches.ToList()).ForEach(s => UpdateSearchStatus(s.codeSearch, CheckStatus(s), null));

            //        return new WebResult<List<SearchDTO>>
            //        {
            //            Message = "רשימת מטלות עודכנה בהצלחה",
            //            Status = true,
            //            Value = SearchCast.GetSearchesDTO(db.Searches.ToList())
            //        };
            //    }
            //}
            //}
        }
    }
}
