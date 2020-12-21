using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookModel
{
    public partial class PhoneNumbersEntities
    {
        public void SaveChangesClientWins()
        {
            CommitChanges(ClientWins: true);
        }

        public void SaveChangesDatabaseStoreWins()
        {
            CommitChanges(ClientWins: false);
        }

        private void CommitChanges(bool ClientWins = true)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    this.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    foreach (var entry in ex.Entries.Where(e => e.State != EntityState.Unchanged))
                    {
                        if (ClientWins)
                        {
                            DbPropertyValues dbValues = entry.GetDatabaseValues();
                            if (dbValues != null)
                            {
                                entry.OriginalValues.SetValues(dbValues);
                            }
                            else
                            {
                                entry.State = EntityState.Added;
                            }
                        }
                        else
                        {
                            entry.Reload();
                        }
                    }
                }
            } while (saveFailed);
        }
    }
}