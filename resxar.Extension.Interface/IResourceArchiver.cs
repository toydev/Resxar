// Resx Archiver Extension Interface
// https://github.com/toydev/Resxar
//
// Copyright (C) 2014 toydev All Rights Reserved.
//
// This software is released under Microsoft Public License(Ms-PL).

using System;
using System.Collections.Generic;
using System.Resources;

namespace resxar.Extension.Interface
{
    /// <summary>
    /// リソースアーカイバーが実装すべきインターフェイスを規定します。
    /// </summary>
    public interface IResourceArchiver
    {
        /// <summary>
        /// リソースを resx に書き込んだことを通知するためのイベントです。
        /// 標準のクライアントは通知内容を標準出力に出力するためにこのイベントを利用します。
        /// </summary>
        event ResourceArchivedEventHandler ResourceArchived;

        /// <summary>
        /// リソースの書き込みをスキップしたことを通知するためのイベントです。
        /// 標準のクライアントは通知内容を標準出力に出力するためにこのイベントを利用します。
        /// </summary>
        event ResourceArchivedEventHandler ResourceArchiveSkipped;

        /// <summary>
        /// 使い方の説明文を返してください。
        /// </summary>
        /// <returns>説明文を返します。クライアントは必要に応じてクライアント自体の説明文の後にリソースアーカイバーの説明を出力してください。</returns>
        string Usage();

        /// <summary>
        /// パラメータを受け取ります。
        /// 標準のクライアントはコマンドライン引数に指定された /paramName1:paramValue1 /paramName2:paramValue2 ... 形式のパラメータを認識し、これが渡されてきます。
        /// </summary>
        /// <param name="parameters">全てのパラメータが格納されたディクショナリです。"in" および "out" もこれに含まれます。</param>
        /// <exception cref="rasxar.ApplicationArgumentException">パラメータに問題がある場合に発生させてください。クライアントは例外のメッセージを表示するとともに異常終了させてください。</exception>
        void SetParameters(IDictionary<string, string> parameters);

        /// <summary>
        /// リソースの取り込みを行います。
        /// ライターを使いリソースを取り込んだ後、ResourceArchived イベントを発生させ、リソースに取り込んだことをクライアントに通視してください。
        /// また、取り込みをスキップする場合はライターへの書き込みを行わずに ResourceArchiveSKipped イベントを発生させ、リソースへの取り込みをスキップしたことをクライアントに通知してください。
        /// クライアントは取り込みの状況をメッセージとして表示してください。
        /// </summary>
        /// <param name="writer">リソースの書き込み使用するライターです。</param>
        /// <param name="resourceFullPath">リソースファイルのフルパスです。</param>
        /// <param name="resourceRelativePath">リソースファイルの入力ルートディレクトリからの相対パスです。</param>
        /// <exception cref="System.ApplicationException">
        /// 全ての処理を中断させる場合に発生させてください。クライアントは例外のメッセージを表示するとともに異常終了させてください。
        /// </exception>
        void AddResource(ResXResourceWriter writer, string resourceFullPath, string resourceRelativePath);
    }
}
