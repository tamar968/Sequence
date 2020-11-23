﻿using DAL;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Casting
{
    class ShopCast
    {
       
        //Convert from shop to shopDTO
        public static ShopDTO GetShopDTO(Shop shop)
        {
            var cat = CategoryCast.GetCategoriesDTO(shop.Category_to_shop.Select(s => s.Category).ToList());
            return new ShopDTO()
            {
                codeShop = shop.codeShop,
                nameShop = shop.nameShop,
                passwordShop = shop.passwordShop,
                phoneShop = shop.phoneShop,
                longitude=shop.longitude,
                latitude=shop.latitude,
                fromHour=shop.fromHour,
                toHour=shop.toHour,
                mailShop=shop.mailShop,
                addressString=shop.addressString,
                Categories=cat,
                isFavorite = shop.isFavorite
            };
        }
        //Convert from ShopDTO to Shop
        public static Shop GetShop(ShopDTO shop)
        {
            return new Shop()
            {
                codeShop = shop.codeShop,
                nameShop = shop.nameShop,
                passwordShop = shop.passwordShop,
                phoneShop = shop.phoneShop,
                mailShop=shop.mailShop,
                latitude=shop.latitude,
                longitude=shop.longitude,
                fromHour=shop.fromHour,
                toHour=shop.toHour,
                addressString=shop.addressString,
                 isFavorite = shop.isFavorite
            };
        }
        //Conver a list of shops to list of shopDTOs
        public static List<ShopDTO> GetShopsDTO(List<Shop> shops)
        {
            List<ShopDTO> shopDTOs = new List<ShopDTO>();
            foreach (var item in shops)
            {
                shopDTOs.Add(GetShopDTO(item));
            }
            return shopDTOs;
        }
    }
}
