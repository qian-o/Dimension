﻿using DimensionService.Models.DimensionModels;
using System.Collections.Generic;

namespace DimensionService.Dao.ChatColumn
{
    public interface IChatColumnDAO
    {
        bool AddChatColumn(string userID, string friendID, string chatID);

        List<ChatColumnModel> GetChatColumns(string userID);
    }
}
