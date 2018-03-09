using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using PirateX.Core;

namespace PirateX.Middleware
{
    public class MidLetterService<TMidLetterRepository,TMidSystemLetterRepository,TLetter,TSystemLetter> : ServiceBase
        where TLetter : class, ILetter
        where TSystemLetter : class, ISystemLetter
        where TMidLetterRepository: MidLetterRepository<TLetter>
        where TMidSystemLetterRepository : MidSystemLetterRepository<TSystemLetter>
    {
        public virtual IEnumerable<IArchiveToLetter> ArchiveToLetters { get; set; }

        /// <summary>
        /// 发送信件
        /// </summary>
        /// <param name="letter"></param>
        public virtual int Send(TLetter letter)
        {
            return base.Resolver.Resolve<TMidLetterRepository>().Insert(letter);
        }
        /// <summary>
        /// 发送信件
        /// </summary>
        /// <param name="letters"></param>
        public virtual void Send(IEnumerable<TLetter> letters)
        {
            base.Resolver.Resolve<TMidLetterRepository>().Insert(letters);
        }

        /// <summary>
        /// 获取信件列表,默认50条
        /// 如果需要多国语言的支持 需要在获取后调用 ConvertLetter
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public virtual IEnumerable<TLetter> GetLetters(int rid, int page, DateTime roleCreateAtUtc,int size = 50)
        {
            var repo = base.Resolver.Resolve<TMidLetterRepository>();

            int maxSystemId = 0;

            List<ILetter> letters = new List<ILetter>();

            //TODO 这边还需要考虑红点的问题

            var config = base.Resolver.Resolve<IDistrictConfig>();

            var sysletters = base.Container.ServerIoc.Resolve<TMidSystemLetterRepository>().GetSystemLetters()
                .Where(item => item.OpenAt < roleCreateAtUtc 
                && (item.TargetDidList == null || item.TargetDidList.Contains(config.Id)) 
                && item.Id>=maxSystemId);

            if (sysletters.Any())
            {
                foreach (var letter in sysletters)
                    letters.Add(letter.ToLetter(rid));
            }

            //这里查看其他系统是否需要生成信件
            if (ArchiveToLetters != null && ArchiveToLetters.Any())
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug($"ArchiveToLetter ->{rid}");

                foreach (var toLetter in ArchiveToLetters)
                    letters.Add(toLetter.Builder<TLetter>(rid));
            }
            if (letters.Any())
                repo.Insert(letters);

            //TODO letter record

            return repo.GetList(rid, page, size);
        }

        /// <summary>
        /// 转换信件的多语言字段
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public virtual TLetter ConvertLetter(TLetter letter, string culture)
        {
            if (letter.FromRid > 0)
                return letter;

            if (letter.i18n != null && letter.i18n.Any())
            {
                var i18n = letter.i18n.First(item => Equals(item.Language.ToLower(), culture.ToLower()));
                if (i18n == null)
                    i18n = letter.i18n.First(item => Equals(item.Language.ToLower(), ""));

                if (i18n != null)
                {
                    letter.Title = i18n.Title;
                    letter.Content = i18n.Content;
                }
            }

            return letter;
        }
        /// <summary>
        /// 转换信件的多语言字段
        /// </summary>
        /// <param name="letters"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public virtual IEnumerable<TLetter> ConvertLetter(IEnumerable<TLetter> letters, string culture)
        {
            foreach (var letter in letters)
                ConvertLetter(letter, culture);

            return letters;
        }

        /// <summary>
        /// 删除信件
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int Delete(long rid, int id)
        {
            return base.Resolver.Resolve<TMidLetterRepository>().Delete(id);
        }

        /// <summary>
        /// 标记信件为已查看
        /// </summary>
        /// <typeparam name="TLetter"></typeparam>
        /// <param name="id"></param>
        public virtual void Read(int id)
        {
            base.Resolver.Resolve<TMidLetterRepository>().SetRead(id);
        }
    }
}
