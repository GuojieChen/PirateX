using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using PirateX.Core;

namespace PirateX.Middleware
{
    public class MidLetterService<TMidLetterRepository,TLetter,TSystemLetter> : ServiceBase
        where TLetter : class, ILetter
        where TSystemLetter : class, ISystemLetter
        where TMidLetterRepository: MidLetterRepository<TLetter, TSystemLetter>
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
        public virtual IEnumerable<TLetter> GetLetters(int rid, int page, DateTime roleCreateAtUtc, int size = 50)
        {
            var repo = base.Resolver.Resolve<TMidLetterRepository>();

            List<TLetter> letters = new List<TLetter>();

            //获取gm系统信件
            //TODO 这边还需要考虑红点的问题

            var sysletters = repo.GetSystemLetters().Where(item => item.OpenAt < roleCreateAtUtc);
            if (sysletters.Any())
            {
                foreach (var letter in sysletters)
                    letters.Add(letter.ToLetter<TLetter>(rid));
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

            return repo.GetList(rid, page, size);
        }


        public virtual TLetter ConvertLetter(TLetter letter, string culture)
        {
            if (letter.FromRid > 0)
                return letter;

            if (letter.i18NTitle != null && letter.i18NTitle.Any())
            {
                var i18n = letter.i18NTitle.First(item => Equals(item.Language.ToLower(), culture.ToLower()));
                if (i18n == null)
                    i18n = letter.i18NTitle.First(item => Equals(item.Language.ToLower(), ""));

                if (i18n != null)
                    letter.Title = i18n.Content;
            }

            if (letter.i18nContent != null && letter.i18nContent.Any())
            {
                var i18n = letter.i18nContent.First(item => Equals(item.Language.ToLower(), culture.ToLower()));
                if (i18n == null)
                    i18n = letter.i18nContent.First(item => Equals(item.Language.ToLower(), ""));

                if (i18n != null)
                    letter.Content = i18n.Content;
            }

            return letter;
        }

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
