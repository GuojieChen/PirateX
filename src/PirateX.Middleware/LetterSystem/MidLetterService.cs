using System.Collections.Generic;
using System.Linq;
using Autofac;
using PirateX.Core;

namespace PirateX.Middleware
{
    public class MidLetterService:ServiceBase
    {
        public IEnumerable<IArchiveToLetter> ArchiveToLetters { get; set; }

        /// <summary>
        /// 发送信件
        /// </summary>
        /// <param name="letter"></param>
        public int Send(ILetter letter)
        {
            return base.Resolver.Resolve<MidLetterRepository>().Insert(letter);
        }

        public void Send(IEnumerable<ILetter> letters)
        {
            base.Resolver.Resolve<MidLetterRepository>().Insert(letters);
        }

        /// <summary>
        /// 获取信件列表,默认50条
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<TLetter> GetLetters<TLetter>(int rid,int page, int size = 50) where TLetter : ILetter
        {
            //这里查看其他系统是否需要生成信件
            if (ArchiveToLetters.Any())
            {
                if(Logger.IsDebugEnabled)
                    Logger.Debug($"ArchiveToLetter ->{rid}");

                foreach (var toLetter in ArchiveToLetters)
                    toLetter.Builder(rid);
            }

            return base.Resolver.Resolve<MidLetterRepository>().GetList<TLetter>(rid, page, size);
        }

        /// <summary>
        /// 删除信件
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete<TLetter>(long rid, int id)
        {
            return base.Resolver.Resolve<MidLetterRepository>().Delete<TLetter>(rid, id);
        }

        /// <summary>
        /// 标记信件为已查看
        /// </summary>
        /// <typeparam name="TLetter"></typeparam>
        /// <param name="id"></param>
        public void Read<TLetter>(int id)
        {
            base.Resolver.Resolve<MidLetterRepository>().SetRead<TLetter>(id);
        }
    }
}
