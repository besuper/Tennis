using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.Objects;

namespace Tennis.DAO
{
    public class OpponentDAO : DAO<Opponent>
    {
        public override bool Create(Opponent obj)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Opponents output INSERTED.ID_OPPONENT DEFAULT VALUES", DatabaseManager.GetConnection()))
            {

                int modified = (int)cmd.ExecuteScalar();

                obj.Id = modified;

                return true;
            }
        }

        public bool AddPlayer(Opponent opponent, Player player)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO PlayersOpponent(id_opponent, id_player) VALUES(@id_opponent, @id_player)", DatabaseManager.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id_opponent", opponent.Id);
                cmd.Parameters.AddWithValue("@id_player", player.Id);

                cmd.ExecuteNonQuery();

                return true;
            }
        }

        public override bool Delete(Opponent obj)
        {
            throw new NotImplementedException();
        }

        public override Opponent Find(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(Opponent obj)
        {
            throw new NotImplementedException();
        }
    }
}
