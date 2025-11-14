using System.Collections.Concurrent;
using PrototypeUserService.Contracts;

namespace PrototypeUserService.Services;

public class MockUserService
{
    // username → userId
    private readonly ConcurrentDictionary<string, Guid> _users = new();

    // token → userId
    private readonly ConcurrentDictionary<string, Guid> _tokens = new();

    // userId → profiel
    private readonly ConcurrentDictionary<Guid, (string Username, int Wins, int Losses)> _profiles = new(); 
    
    // userId -> owned cards
    private readonly ConcurrentDictionary<Guid, List<Card>> _ownedCards = new();

    // userId -> decks
    private readonly ConcurrentDictionary<Guid, List<Deck>> _decks = new();
    
    private readonly Random _random = new();

    // Simuleert registratie
    public Guid Register(string username, string password)
    {
        var id = _users.GetOrAdd(username, _ => Guid.NewGuid());
        _profiles.TryAdd(id, (username, 0, 0));
        return id;
    }

    // Simuleert login, maakt token aan
    public string? Login(string username, string password)
    {
        if (!_users.TryGetValue(username, out var userId))
            return null;

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..22];
        _tokens[token] = userId;
        return token;
    }

    // Token omzetten naar userId
    public Guid? ResolveUser(string token)
    {
        return _tokens.TryGetValue(token, out var id) ? id : null;
    }

    // Optioneel: alle users tonen (debug)
    public IEnumerable<(string Username, Guid Id)> ListUsers()
        => _users.Select(kv => (kv.Key, kv.Value));
    
    public (Guid Id, string Username, int Wins, int Losses)? GetProfile(Guid id) // 🟩
    {
        if (_profiles.TryGetValue(id, out var p))
            return (id, p.Username, p.Wins, p.Losses);
        return null;
    }

    public bool UpdateProfile(Guid id, string? username, int? wins, int? losses) // 🟩
    {
        if (!_profiles.TryGetValue(id, out var current)) return false;

        var newUsername = username ?? current.Username;
        var newWins     = wins     ?? current.Wins;
        var newLosses   = losses   ?? current.Losses;

        // username-wijziging ook doorvoeren in _users (omgekeerde lookup simplificeren we)
        if (!string.Equals(newUsername, current.Username, StringComparison.Ordinal))
        {
            // simpele heropbouw: verwijder oude username -> id en voeg nieuwe toe
            _users.TryRemove(current.Username, out _);
            _users[newUsername] = id;
        }

        _profiles[id] = (newUsername, newWins, newLosses);
        return true;
    }
    
    // -------------------------------
    // Kaartbeheer
    // -------------------------------
    public List<Card> GetOwnedCards(Guid userId)
    {
        return _ownedCards.GetOrAdd(userId, _ => new List<Card>());
    }

    public void AddOwnedCard(Guid userId, string cardName)
    {
        var cards = _ownedCards.GetOrAdd(userId, _ => new List<Card>());
        cards.Add(new Card(Guid.NewGuid(), cardName));
    }

// -------------------------------
// Deckbeheer
// -------------------------------
    public Deck CreateDeck(Guid ownerId, string name)
    {
        var decks = _decks.GetOrAdd(ownerId, _ => new List<Deck>());
        var deck = new Deck(Guid.NewGuid(), ownerId, name, new List<Guid>());
        decks.Add(deck);
        return deck;
    }

    public IEnumerable<Deck> GetDecks(Guid ownerId)
    {
        return _decks.GetOrAdd(ownerId, _ => new List<Deck>());
    }

    public Deck? ModifyDeckCards(Guid ownerId, Guid deckId, List<Guid>? add, List<Guid>? remove)
    {
        if (!_decks.TryGetValue(ownerId, out var decks)) return null;
        var deck = decks.FirstOrDefault(d => d.Id == deckId);
        if (deck == null) return null;

        if (add != null)
            deck.CardIds.AddRange(add);

        if (remove != null)
            deck.CardIds.RemoveAll(id => remove.Contains(id));

        return deck;
    }


}