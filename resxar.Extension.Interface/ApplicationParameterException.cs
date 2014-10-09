// Resx Archiver Extension Interface
// https://github.com/toydev/Resxar
//
// Copyright (C) 2014 toydev All Rights Reserved.
//
// This software is released under Microsoft Public License(Ms-PL).

using System;

namespace resxar
{
    /// <summary>
    /// アプリケーションのパラメータが不足していることを表す例外です。
    /// </summary>
    public class ApplicationParameterException : ApplicationException
    {

        public ApplicationParameterException(string message)
            : base(message)
        {
        }
    }
}
