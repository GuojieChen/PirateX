using System.Collections.Generic;
using PirateX.Core;

namespace PirateX.Middleware
{
    public class MidLetterService:ServiceBase

    {
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
        public List<TLetter> GetLetters<TLetter>(long rid,int page, int size = 50) where TLetter : ILetter
        {
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
