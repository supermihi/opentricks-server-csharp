using Doppelkopf.API;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Tests;

public static class TestData
{
  public static class Rules
  {
    public static readonly Doppelkopf.Server.Model.Rules Default = new(RuleSet.Minikopf, 4);
  }

  public static class Meta
  {
    public static readonly TableMeta Default = TableMeta.Create("default", Users.Owner, Rules.Default);
  }

  public static class Users
  {
    public static UserId Owner = new("owner");
    public static UserId Member1 = new("member1");
    public static UserId Member2 = new("member2");
    public static UserId Member3 = new("member3");
    public static UserId NotMember = new("not member");

    public static readonly TableUsers Single = TableUsers.Init(new[] { Owner });
    public static readonly TableUsers Full = TableUsers.Init(new[] { Owner, Member1, Member2, Member3 });
  }

  public static class Table
  {
    public static readonly Model.Table Initial = Model.Table.Init(Meta.Default);
    public static readonly Model.Table JustStarted = new(Meta.Default, Users.Full, 1, Session.JustStarted);
  }

  public static class Session
  {
    public static readonly Doppelkopf.Sessions.Session JustStarted = Sessions.Session.Init(
      Rules.Default.ToConfiguration(),
      Rules.Default.MaxSeats,
      null);
  }
}
