namespace Domain.Dto;

public class PagedResponse<T>
    {
        public IList<T> Data { get; set; } = default!;

        private string? _nextToken;

        public string? NextToken
        {
            get => _nextToken;
            set => _nextToken = value != "%7b%7d" ? value : null;
        }

        public int Limit { get; set; }
        public string? PreviousToken { get; set; }
    }
