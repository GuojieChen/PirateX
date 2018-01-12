using System.Collections.Generic;
using System.Linq;
using PirateX.Core;
using PirateX.Middleware.LetterSystem;

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
            using (var uow = base.CreateUnitOfWork())
            {
                var repo = uow.Repository<LetterRepository>();
                return repo.Insert(letter);   
            }
        }

        public void Send(IEnumerable<ILetter> letters)
        {
            using (var uow = base.CreateUnitOfWork())
            {
                var repo = uow.Repository<LetterRepository>();
                repo.Insert(letters);
            }
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

            using (var uow = base.CreateUnitOfWork())
            {
                var repo = uow.Repository<LetterRepository>();
                return repo.GetList<TLetter>(rid, page, size);
            }
        }

        /// <summary>
        /// 删除信件
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(long rid, int id)
        {
            using (var uow = base.CreateUnitOfWork())
            {
                var repo = uow.Repository<LetterRepository>();
                return repo.Delete(rid, id);
            }
        }
    }
}
