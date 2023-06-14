namespace RoyalCode.OpearationResult.TestApi.Application.Pizzas
{
    /// <summary>
    /// <para>
    ///     Estoque de ingredientes.
    /// </para>
    /// </summary>
    public class Estoque : BaseEntity
    {
        /// <summary>
        /// Ingrediente.
        /// </summary>
        public required Ingrediente Ingrediente { get; set; }

        /// <summary>
        /// Quantidade do ingrediente.
        /// </summary>
        public required int Quantidade { get; set; }

        /// <summary>
        /// Unidade de medida do ingrediente.
        /// </summary>
        public required string Unidade { get; set; }
    }
}
